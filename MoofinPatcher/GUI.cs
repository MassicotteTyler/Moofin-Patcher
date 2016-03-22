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

        public GUI()
        {
            InitializeComponent();
        }

        private void updateButton_click(object sender, EventArgs e)
        {
            downloadFile();
        }

        private void fileButton_Click(object sender, EventArgs e)
        {

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
            try
            {
                ZipFile.ExtractToDirectory("mods.zip", textBox1.Text);

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
            catch (Exception e)
            {
                statusLabel.Text = "Extracting failed";
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
