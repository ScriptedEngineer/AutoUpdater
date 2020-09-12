using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Updater
{
    public class MyExtentions
    {
        public static void AsyncWorker(Action act) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, act);
        public static string RegexMatch(string source, string regex)
        {
            Match Mxx = Regex.Match(source, regex);
            if (Mxx.Success) return Mxx.Groups[1].Value;
            else return null;
        }
    }
    public class MyStatic
    {
        public static UpdateType Type = UpdateType.None;
        public static string LinkOrToken = "";//ScriptedEngineer/SE-BlueprintEditor
        public static string RunAfterUpdate = "";
        static bool? AppRunned = null;
        public static string DownloadURL = null;
        static string SomeData = "";
        public static string LogLanguage = null;
        public static bool CheckVersion(string ver, out string lastVersion)
        {
            if (string.IsNullOrEmpty(SomeData)) GetSomeData();
            lastVersion = "";
            switch (Type)
            {
                case UpdateType.None: return false;
                case UpdateType.GitHubReleases:
                    lastVersion = MyExtentions.RegexMatch(SomeData, @"""tag_name"":""([^""]*)""");
                    string[] VFc = ver.Split('.');
                    string[] VFl = lastVersion.Split('.');
                    bool oldVer = false;
                    for (int i = 0; i < VFc.Length; i++)
                    {
                        if (VFc[i] != VFl[i])
                        {
                            int.TryParse(VFc[i], out int VFci);
                            int.TryParse(VFl[i], out int VFli);
                            if (VFli > VFci) oldVer = true;
                        }
                    }
                    return oldVer;
                case UpdateType.XMLServer:
                    break;
                case UpdateType.WSXZApi:
                    break;
            }
            return false;
        }
        public static string GetUpdateLog(string version)
        {
            if (string.IsNullOrEmpty(SomeData)) GetSomeData();
            switch (Type)
            {
                case UpdateType.GitHubReleases:
                    string updLog = "";
                    string[] gVersions = SomeData.Split(new string[] { @"},{""url"":" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string gVer in gVersions)
                    {
                        string vsi = MyExtentions.RegexMatch(gVer, @"""tag_name"":""([^""]*)""");
                        if (vsi == version) break;
                        string[] Lines = MyExtentions.RegexMatch(gVer, @"""body"":""([^""]*)""").Split(new string[] { "\\r\\n" }, StringSplitOptions.RemoveEmptyEntries);
                        bool writes = true;
                        foreach (var Line in Lines)
                        {
                            if (Line.StartsWith("# "))
                            {
                                if (!string.IsNullOrEmpty(LogLanguage))
                                {
                                    string[] lang = Line.Split(']');
                                    if (lang[0] == "# [" + LogLanguage) writes = true;
                                    else writes = false;
                                    if (lang.Length == 1) writes = true;
                                }
                                if (writes) updLog += "\r\n" + Line + "\r\n";
                            }
                            else
                            if (Line.StartsWith("### "))
                            {
                                //Ingore
                            }
                            else
                            {
                                if (writes) updLog += Line + "\r\n";
                            }
                        }
                    }
                    return updLog.Trim();
                case UpdateType.XMLServer:
                    break;
                case UpdateType.WSXZApi:
                    break;
            }
            return "";
        }
        public static void GetSomeData()
        {
            switch (Type)
            {
                case UpdateType.GitHubReleases:
                    using (var client = new System.Net.WebClient())
                    {
                        client.Headers.Add("User-Agent", "ScriptedUpdater");
                        client.Encoding = Encoding.UTF8;
                        SomeData = client.DownloadString($"https://api.github.com/repos/{LinkOrToken}/releases");
                        DownloadURL = MyExtentions.RegexMatch(SomeData, @"""browser_download_url"":""([^""]*)""");
                    }
                    break;
                case UpdateType.XMLServer:
                    break;
                case UpdateType.WSXZApi:
                    break;
            }
        }
        public static void RunApp(bool updated = false)
        {
            if (!AppRunned.HasValue || !AppRunned.Value)
            {
                try
                {
                    Process.Start(RunAfterUpdate, updated?"Updated":"RemindLater");
                    AppRunned = true;
                }
                catch
                {
                    if(!AppRunned.HasValue)
                        MessageBox.Show("updater couldn't run program", "Update error", MessageBoxButton.OK, MessageBoxImage.Error);
                    AppRunned = false;
                }
                finally
                {
                    Application.Current.Shutdown();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
    public enum UpdateType
    {
        None,
        GitHubReleases,
        XMLServer,
        WSXZApi
    }
}
