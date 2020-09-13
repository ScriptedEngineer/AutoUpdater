using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        static readonly INIManager UpdaterINI = new INIManager("Updater.ini");
        public MainWindow()
        {
            DownLoader = new Downloader();
            
            InitializeComponent();
            ReadINIFile();
            ParseComandline();
        }
        private void ReadINIFile()
        {
            if (File.Exists("Updater.ini"))
            {
                DownLoader.Title = Title = UpdaterINI.GetPrivateString("UI", "Title", Title);
                WN.Content = UpdaterINI.GetPrivateString("UI", "WhatNew", WN.Content.ToString());
                RemindMeLater.Content = UpdaterINI.GetPrivateString("UI", "RemindLater", RemindMeLater.Content.ToString());
                UpdateNow.Content = UpdaterINI.GetPrivateString("UI", "UpdateNow", UpdateNow.Content.ToString());
                AvailableVer.Content = UpdaterINI.GetPrivateString("UI", "Available", AvailableVer.Content.ToString());
                CurrentVer.Content = UpdaterINI.GetPrivateString("UI", "Current", CurrentVer.Content.ToString());
                DownLoader.PleaseWait.Content = UpdaterINI.GetPrivateString("UI", "PleaseWait", DownLoader.PleaseWait.Content.ToString());
                Downloader.DownloadingUpdate = UpdaterINI.GetPrivateString("UI", "DownloadingFile", Downloader.DownloadingUpdate);
                Downloader.ExtractUpdate = UpdaterINI.GetPrivateString("UI", "ExtractingUpdate", Downloader.ExtractUpdate);
                switch (UpdaterINI.GetPrivateString("Updates", "Type"))
                {
                    case "GitHub":
                        MyStatic.Type = UpdateType.GitHubReleases;
                        break;
                    case "XMLS":
                        MyStatic.Type = UpdateType.XMLServer;
                        break;
                    case "API":
                        MyStatic.Type = UpdateType.WSXZApi;
                        break;
                    case "None":
                        MyStatic.Type = UpdateType.None;
                        break;
                }
                MyStatic.LinkOrToken = UpdaterINI.GetPrivateString("Updates", "LinkORToken");
                MyStatic.RunAfterUpdate = UpdaterINI.GetPrivateString("Updates", "Run");
                MyStatic.RunAfterUpdateParams = UpdaterINI.GetPrivateString("Updates", "RunParams");
                MyStatic.LogLanguage = UpdaterINI.GetPrivateString("UI", "LogLang");
                if(UpdaterINI.GetPrivateString("UI", "DarkTheme") == "1")
                {
                    var uri = new Uri("DarkTheme.xaml", UriKind.Relative);
                    ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                }
            }
        }
        private void WriteINIFile()
        {
            UpdaterINI.WritePrivateString("UI", "Title", Title);
            UpdaterINI.WritePrivateString("UI", "WhatNew", WN.Content.ToString());
            UpdaterINI.WritePrivateString("UI", "RemindLater", RemindMeLater.Content.ToString());
            UpdaterINI.WritePrivateString("UI", "UpdateNow", UpdateNow.Content.ToString());
            UpdaterINI.WritePrivateString("UI", "Available", AvailableVer.Content.ToString());
            UpdaterINI.WritePrivateString("UI", "Current", CurrentVer.Content.ToString());
            UpdaterINI.WritePrivateString("UI", "PleaseWait", DownLoader.PleaseWait.Content.ToString());
            UpdaterINI.WritePrivateString("UI", "DownloadingFile", Downloader.DownloadingUpdate);
            UpdaterINI.WritePrivateString("UI", "ExtractingUpdate", Downloader.ExtractUpdate);
            UpdaterINI.WritePrivateString("UI", "DarkTheme", Application.Current.Resources.MergedDictionaries.Count.ToString());
            UpdaterINI.WritePrivateString("UI", "LogLang", MyStatic.LogLanguage);
            UpdaterINI.WritePrivateString("Updates", "LinkORToken", MyStatic.LinkOrToken);
            UpdaterINI.WritePrivateString("Updates", "Run", MyStatic.RunAfterUpdate);
            UpdaterINI.WritePrivateString("Updates", "RunParams", MyStatic.RunAfterUpdateParams);
            switch (MyStatic.Type)
            {
                case UpdateType.GitHubReleases:
                    UpdaterINI.WritePrivateString("Updates", "Type", "GitHub");
                    break;
                case UpdateType.XMLServer:
                    UpdaterINI.WritePrivateString("Updates", "Type", "XMLS");
                    break;
                case UpdateType.WSXZApi:
                    UpdaterINI.WritePrivateString("Updates", "Type", "API");
                    break;
                case UpdateType.None:
                    UpdaterINI.WritePrivateString("Updates", "Type", "None");
                    UpdaterINI.WritePrivateString("Updates", "LinkORToken", "Undefined");
                    UpdaterINI.WritePrivateString("Updates", "Run", "Undefined");
                    break;
            }
        }
        private void ParseComandline()
        {
            string version = "";
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
                "/JustDownload", "/LogLang", "/Version",
                "/RunApp" , "/RunAppParams",
                "/DarkTheme"
            };
            foreach (var arg in args)
            {
                if (allowedParams.Contains(arg))
                {
                    switch (arg)
                    {
                        //Методы обновления
                        case "/GitHub": //GitHub Releases
                            MyStatic.Type = UpdateType.GitHubReleases;
                            break;
                        case "/XMLS":   //XMLServer (Не сделан)
                            MyStatic.Type = UpdateType.XMLServer;
                            break;
                        case "/API":    //wsxz.ru/api/ (Не сделан)
                            MyStatic.Type = UpdateType.WSXZApi;
                            break;

                        //Настройки
                        case "/JustDownload": //Только скачивание и распаковывание
                            Button_Click(UpdateNow, new RoutedEventArgs());
                            return;
                        case "/DarkTheme":
                            var uri = new Uri("DarkTheme.xaml", UriKind.Relative);
                            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                            Application.Current.Resources.MergedDictionaries.Clear();
                            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                            break;
                    }
                    next = arg;
                }
                else if (!string.IsNullOrEmpty(next))
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
                        case "/GitHub":
                        case "/XMLS":
                        case "/API":
                            MyStatic.LinkOrToken = arg;
                            break;

                        //Настройки
                        case "/LogLang": //Язык логов
                            MyStatic.LogLanguage = arg;
                            break;
                        case "/Version": //Версия
                            version = arg;
                            break;
                        case "/RunApp": //Что запускать
                            MyStatic.RunAfterUpdate = arg;
                            break;
                        case "/RunAppParams": //Параметры запуска
                            MyStatic.RunAfterUpdateParams = arg;
                            break;
                    }
                    next = "";
                }
            }
            if (MyStatic.Type == UpdateType.None)
                MyStatic.RunApp();
            WriteINIFile();
            //Если версия не указана берем ее сами
            if (string.IsNullOrEmpty(version))
            {
                if (File.Exists(MyStatic.RunAfterUpdate))
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(MyStatic.RunAfterUpdate);
                    version = versionInfo.ProductVersion;
                }
            }


            if (!MyStatic.CheckVersion(version, out string lV) && File.Exists(MyStatic.RunAfterUpdate))
            {
                MyStatic.RunApp();
            }
            else
            {
                AvailableVer.Content += " - " + lV;
                if (!string.IsNullOrEmpty(version))
                    CurrentVer.Content += " - " + version;
                else CurrentVer.Content = "";
                UpdLog.Text = MyStatic.GetUpdateLog(version);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            DownLoader.Show();
            DownLoader.StartDownload();
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
