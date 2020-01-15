using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;

namespace HEIFConverter
{
    public partial class Form1 : Form
    {
        bool deleteHEIC;
        string path;
        int fileCount;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = false;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                listBox1.Items.Clear();
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                path = folderBrowserDialog1.SelectedPath;
                textBox2.AppendText("Fetching list of convertable .HEIC files");
                textBox2.AppendText(Environment.NewLine);
                PopulateListBox(listBox1, folderBrowserDialog1.SelectedPath, "*.heic");
                fileCount = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.heic", SearchOption.TopDirectoryOnly).Length;
                textBox2.AppendText(fileCount + " files ready to be converted");
                textBox2.AppendText(Environment.NewLine);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked == true)
            {
                deleteHEIC = true;
                textBox2.AppendText("Will delete old .HEIC files after conversion");
                textBox2.AppendText(Environment.NewLine);
            } else { 
                deleteHEIC = false;
                textBox2.AppendText("Will keep old .HEIC files after conversion");
                textBox2.AppendText(Environment.NewLine);
            }
        }

        private void PopulateListBox(ListBox lsb, string Folder, string FileType)
        {
            DirectoryInfo dinfo = new DirectoryInfo(Folder);
            FileInfo[] Files = dinfo.GetFiles(FileType);
            foreach (FileInfo file in Files)
            {
                lsb.Items.Add(file.Name);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.AppendText("Program loaded.");
            textBox2.AppendText(Environment.NewLine);
            deleteHEIC = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(path))
            {
                ProcessFile(path);
            } else if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            } else
            {
                textBox2.AppendText("Not a valid file or directory.");
                textBox2.AppendText(Environment.NewLine);
            }
            textBox2.AppendText("Successfully converted " + fileCount + " files to JPEG.");
            MessageBox.Show("Thanks for using HEIFConverter! You converted " + fileCount + " .HEIC files to .JPEG!");
            textBox2.AppendText(Environment.NewLine);
        }

        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (Path.GetExtension(fileName).ToLower() == ".heic")
                {
                    ProcessFile(fileName);
                    if (deleteHEIC == true)
                    {
                        File.Delete(fileName);
                    }
                   
                }
            }


            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        public void ProcessFile(string path)
        {
            using (MagickImage image = new MagickImage(path))
            {
                string newFile = path.Replace(Path.GetExtension(path), ".jpg");
                image.Write(newFile);
            }
            textBox2.AppendText("Converted file '" + path + "'");
            textBox2.AppendText(Environment.NewLine);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/danieljlawson/HEIFConverter");
        }
    }
}
