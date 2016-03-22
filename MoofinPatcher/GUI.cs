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
        private string minecraftDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\AppData\\Roaming\\.minecraft\\mods";
        private string DOWNLOAD_PATH;
        public GUI()
        {
            InitializeComponent();
            textBox.Text = minecraftDir;
            DOWNLOAD_PATH = Path.GetTempPath() + "mods.zip";
        }

        private void updateButton_click(object sender, EventArgs e)
        {
            updateButton.Enabled = false;
            downloadFile();
            
        }

        private void fileButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = minecraftDir;
            if (fbd.ShowDialog() != DialogResult.OK)
                return;
            textBox.Text = fbd.SelectedPath;
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
                wc.DownloadFileAsync(new Uri(packLink), DOWNLOAD_PATH);
            }
        }

        private void extractFiles()
        {
            statusLabel.Text = "Extracting files";
            ZipArchive files = null;
            try
            {
                files = ZipFile.Open(DOWNLOAD_PATH, ZipArchiveMode.Read);
                overWrite(files, textBox.Text);
                statusLabel.Text = "Done";
                files.Dispose();
                File.Delete(DOWNLOAD_PATH);
                updateButton.Enabled = true;


            } catch (DirectoryNotFoundException)
            {
                statusLabel.Text = "Output Directory not found";
            }
            catch (PathTooLongException)
            {
                statusLabel.Text = "OutputPath too long";
            }
            catch (UnauthorizedAccessException)
            {
                statusLabel.Text = "Unauthorized access to output directory";
            }
            catch (IOException)
            {
                statusLabel.Text = "Extracting failed Input/Output Error";
            }
            catch (Exception e)
            {
                statusLabel.Text = "Extracting failed";
                Console.WriteLine(e.ToString());
            } finally
            {
                updateButton.Enabled = true;
            }

        }

        private void overWrite(ZipArchive files, string destDir)
        {
            foreach (ZipArchiveEntry file in files.Entries)
            {
                string completeFileName = Path.Combine(destDir, file.FullName);
                if (file.Name == "")
                {//File exists so skip
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
            statusLabel.Text = "Done Downloading";
            extractFiles();

        }
    }
}
