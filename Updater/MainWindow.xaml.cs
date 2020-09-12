using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Updater
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Downloader DownLoader; 
        public MainWindow()
        {
            DownLoader = new Downloader();
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            string next = "";
            List<string> allowedParams = new List<string> { 
                //Команды замены текстов UI
                "/Title", "/WhatNew", "/RemindLater", "/UpdateNow",
                "/Available", "/Current", "/PleaseWait", "/DownloadingFile",
                "/ExtractingUpdate",

                //Методы обновления
                "/GitHub", "/XMLS", "/API",

                //Настройки
                "/JustDownload", "/LogLang", "/Version", "/RunApp"
            };
            foreach (var arg in args)
            {
                if (allowedParams.Contains(arg))
                {
                    switch (arg) {
                        //Методы обновления
                        case "/GitHub": //GitHub Releases
                            MyStatic.Type = UpdateType.GitHubReleases;
                            break;
                        case "/XMLS":   //XMLServer
                            MyStatic.Type = UpdateType.XMLServer;
                            break;
                        case "/API":    //wsxz.ru/api/
                            MyStatic.Type = UpdateType.WSXZApi;
                            break;

                        //Настройки
                        case "/JustDownload": //Только скачивание и распаковывание
                            Button_Click(UpdateNow, new RoutedEventArgs());
                            break;
                    }
                    next = arg;
                }
                else if(!string.IsNullOrEmpty(next))
                {
                    switch (next)
                    {
                        //Команды замены текстов UI
                        case "/Title":
                            DownLoader.Title = Title = arg;
                            break;
                        case "/WhatNew":
                            WN.Content = arg;
                            break;
                        case "/RemindLater":
                            RemindMeLater.Content = arg;
                            break;
                        case "/UpdateNow":
                            UpdateNow.Content = arg;
                            break;
                        case "/Available":
                            AvailableVer.Content = arg;
                            break;
                        case "/Current":
                            CurrentVer.Content = arg;
                            break;
                        case "/PleaseWait":
                            DownLoader.PleaseWait.Content = arg;
                            break;
                        case "/DownloadingFile":
                            Downloader.DownloadingUpdate = arg;
                            break;
                        case "/ExtractingUpdate":
                            Downloader.ExtractUpdate = arg;
                            break;

                        //Методы обновления
                        case "/GitHub": case "/XMLS": case "/API":
                            MyStatic.LinkOrToken = arg;
                            break;

                        //Настройки
                        case "/LogLang": //Язык логов
                            MyStatic.LogLanguage = arg;
                            break;
                        case "/Version": //Версия
                            if (!MyStatic.CheckVersion(arg, out string lV))
                            {
                                MyStatic.RunApp();
                            }
                            else
                            {
                                AvailableVer.Content += " - "+ lV;
                                CurrentVer.Content += " - " + arg;
                                UpdLog.Text = MyStatic.GetUpdateLog(arg);
                            }
                            break;
                        case "/RunApp": //Что запускать
                            MyStatic.RunAfterUpdate = arg;
                            break;
                    }
                    next = "";
                }
            }
            if (MyStatic.Type == UpdateType.None)
                MyStatic.RunApp();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DownLoader.Show();
            DownLoader.StartDownload();
            Hide();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MyStatic.RunApp();
        }

        private void RemindMeLater_Click(object sender, RoutedEventArgs e)
        {
            MyStatic.RunApp();
        }
    }
}
