using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace MoofinPatcher
{
    public partial class GUI : Form
    {
        private const string packLink = "https://www.dropbox.com/s/71idlrnv59mak6z/modz.zip?dl=1";
        private static char sep = Path.DirectorySeparatorChar;
        private string minecraftDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + string.Format("{0}AppData{0}Roaming{0}.minecraft{0}mods", sep);

        public GUI()
        {
            InitializeComponent();
            textBox1.Text = minecraftDir;
        }

        private void updateButton_click(object sender, EventArgs e)
        {
            downloadFile();
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = minecraftDir;
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            textBox1.Text = fbd.SelectedPath;
        }

        public void downloadFile()
        {
            using (WebClient wc = new WebClient())
            {
                statusLabel.Visible = true;
                progressBar.Visible = true;
                statusLabel.Text = "Downloading mods";
                wc.DownloadProgressChanged += dl_DownloadProgressChanged;
                wc.DownloadFileCompleted += downloadComplete;
                wc.DownloadFileAsync(new Uri(packLink), "mods.zip");
            }
        }

        private void extractFiles()
        {
            statusLabel.Text = "Extracting files";
            ZipArchive files = null;
            try
            {
                files = ZipFile.Open("mods.zip", ZipArchiveMode.Read);
                //ZipFile.ExtractToDirectory("mods.zip", textBox1.Text);
                //files.ExtractToDirectory(textBox1.Text);
                overWrite(files, textBox1.Text);
                statusLabel.Text = "Done";


            } catch (DirectoryNotFoundException e)
            {
                statusLabel.Text = "Output Directory not found";
            }
            catch (PathTooLongException e)
            {
                statusLabel.Text = "OutputPath too long";
            }
            catch (UnauthorizedAccessException e)
            {
                statusLabel.Text = "Unauthorized access to output directory";
            }
            catch (IOException e)
            {
                //try
                //{
                //    overWrite(files, textBox1.Text);
                //    statusLabel.Text = "Done";
                //} catch(Exception ex)
                //{
                //    statusLabel.Text = "Extracting failed";
                //    ex.ToString();
                //}
                statusLabel.Text = "Extracting failed";
            }
            catch (Exception e)
            {
                statusLabel.Text = "Extracting failed";
                Console.WriteLine(e.ToString());
            }

        }

        private void overWrite(ZipArchive files, string destDir)
        {
			if(!Directory.Exists(destDir))
				Directory.CreateDirectory(destDir);

            foreach (ZipArchiveEntry file in files.Entries)
            {
                string completeFileName = Path.Combine(destDir, file.FullName);
                if (file.Name == "")
                {// Assuming Empty for Directory
                    //Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }     

        }
        private void dl_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            statusLabel.Text = string.Format("Download: {0} MB / {1} MB",
                (e.BytesReceived / 1024d / 1024d).ToString("0.00"),
                (e.TotalBytesToReceive / 1024d / 1024d).ToString("0.00"));
            progressBar.Value = e.ProgressPercentage;
        }

        private void downloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            statusLabel.Text = "Done";
            extractFiles();

        }
    }
}
