using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Updater
{
    /// <summary>
    /// Логика взаимодействия для Downloader.xaml
    /// </summary>
    public partial class Downloader : Window
    {
        public static string DownloadingUpdate = "Downloading file";
        public static string ExtractUpdate = "Extracting update";
        public Downloader()
        {
            InitializeComponent();
        }

        public void StartDownload()
        {
            Status.Content = $"{DownloadingUpdate}...";
            if (string.IsNullOrEmpty(MyStatic.DownloadURL)) MyStatic.GetSomeData();
            WebClient web = new WebClient();
            web.DownloadDataAsync(new Uri(MyStatic.DownloadURL));
            web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            web.DownloadDataCompleted += new DownloadDataCompletedEventHandler(DownloadDataCompleted);
            Process proc = new Process();
            proc.StartInfo.FileName = "CMD.exe";
            proc.StartInfo.Arguments = "/c taskkill /f /im " + MyStatic.RunAfterUpdate;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            proc.WaitForExit();
        }
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Procents.Content = e.ProgressPercentage + "%";
            Progress.Value = e.ProgressPercentage;
            Status.Content = $"{DownloadingUpdate}... ({e.BytesReceived/1024}kb of {e.TotalBytesToReceive/1024}kb)";
        }
        private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            Status.Content = $"{ExtractUpdate}...";
            Procents.Content = 0.ToString("0.0%");
            Progress.Value = 0;
            new Task(() =>
            {
                Thread.Sleep(400);
                using (MemoryStream Mem = new MemoryStream(e.Result))
                {
                    ZipArchive archive = new ZipArchive(Mem, ZipArchiveMode.Update);
                    //archive.ExtractToDirectory("./");
                    int counter = 0, max = archive.Entries.Count;
                    foreach (ZipArchiveEntry file in archive.Entries)
                    {

                        string completeFileName = System.IO.Path.Combine("./", file.FullName);
                        string directory = System.IO.Path.GetDirectoryName(completeFileName);

                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        if (file.Name != "")
                            file.ExtractToFile(completeFileName, true);

                        counter++;
                        double procents = (double)counter / (double)max;
                        MyExtentions.AsyncWorker(() =>
                        {
                            Procents.Content = procents.ToString("0.0%");
                            Progress.Value = procents * 100;
                        });
                    }
                }
                Thread.Sleep(200);
                MyExtentions.AsyncWorker(() =>
                {
                    Status.Content = "Updated";
                    MyStatic.RunApp();
                });
            }).Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MyStatic.RunApp();
        }
    }
}
