using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ImageMagick;

namespace HEICConverter
{
    public partial class Form1 : Form
    {
        bool deleteHEIC;
        string path;
        int fileCount;
        int convertedCount;
        List<string> failedfilesA = new List<string>();
        bool converting = false;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
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
                textBox2.AppendText("Fetching list of convertable .HEIC files" + Environment.NewLine);
                PopulateListBox(listBox1, folderBrowserDialog1.SelectedPath, "*.heic");
                fileCount = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.heic", SearchOption.TopDirectoryOnly).Length;
                textBox2.AppendText(fileCount + " files ready to be converted" + Environment.NewLine);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                deleteHEIC = true;
                textBox2.AppendText("Will delete old .HEIC files after conversion" + Environment.NewLine);
            }
            else
            {
                deleteHEIC = false;
                textBox2.AppendText("Will keep old .HEIC files after conversion" + Environment.NewLine);
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
            textBox2.AppendText("Program loaded successfully" + Environment.NewLine);
            deleteHEIC = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            converting = true;
            if (File.Exists(path))
            {
                ProcessFile(path);
            }
            else if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }
            else
            {
                textBox2.AppendText("Not a valid file or directory" + Environment.NewLine);
                return;
            }

            converting = false;
            if (failedfilesA.Count() > 0)
            {
                textBox2.AppendText($"Failed to convert {failedfilesA.Count()} file(s)" + Environment.NewLine);
            }
            textBox2.AppendText($"Successfully converted {convertedCount} files to {comboBox1.Text.ToUpper()}" + Environment.NewLine);
            MessageBox.Show($"Thanks for using HeicConverter! You converted {convertedCount} .Heic files to {comboBox1.Text.ToUpper()}!");
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
                        File.Delete(fileName);
                }
            }

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        System.Drawing.Imaging.ImageFormat fileType;
        public bool IsImageOk(string filename)
        {
            try
            {
                Image img = Image.FromFile(filename);
                if (comboBox1.Text == ".jpg")
                    fileType = System.Drawing.Imaging.ImageFormat.Jpeg;
                else
                    fileType = System.Drawing.Imaging.ImageFormat.Png;
                return img.RawFormat.Equals(fileType);
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
        }

        public void ProcessFile(string path)
        {
            try
            {
                string newimagePath;
                using (MagickImage image = new MagickImage(path))
                {
                    string newFile = path.Replace(Path.GetExtension(path), comboBox1.Text);
                    image.Write(newFile);
                    newimagePath = newFile;
                }

                if (IsImageOk(newimagePath))
                {
                    convertedCount++;
                    textBox2.AppendText($"Successfully converted {path} to {comboBox1.Text.ToUpper()}" + Environment.NewLine);
                }
                else
                {
                    failedfilesA.Add(newimagePath);
                    textBox2.AppendText($"Failed to convert file {path}" + Environment.NewLine);
                }

            }
            catch
            {
                failedfilesA.Add(path);
                textBox2.AppendText($"Failed to convert file {path}" + Environment.NewLine);
            }
            
            GC.Collect();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/B0N3head/HEICConverter"); //Deleted profile
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (converting)
            {
                var window = MessageBox.Show("Close the window?", "Are you sure?", MessageBoxButtons.YesNo);
                e.Cancel = (window == DialogResult.No);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            button2.Text = "Convert to " + comboBox1.Text.ToUpper();
            this.Text = $"Heic Converter (To {comboBox1.Text.ToUpper()})";
        }
    }
}
