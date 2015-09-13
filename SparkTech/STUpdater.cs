namespace SparkTech
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Diagnostics.CodeAnalysis; // Remove when .NET 4.6 arrives

    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    // ReSharper disable once InconsistentNaming
    public class STUpdater
    {
        public event onGetVersionCompleted OnGetVersionCompleted;

        // ReSharper disable once InconsistentNaming
        public delegate void onGetVersionCompleted(OnGetVersionCompletedArgs args);

        private WebRequest webRequest;
        private readonly string assemblyName;
        private readonly OnGetVersionCompletedArgs versionCompletedArgs;

        public STUpdater(string pAssemblyName)
        {
            assemblyName = pAssemblyName;
            versionCompletedArgs = new OnGetVersionCompletedArgs();
        }

        public void GetLastVersionAsync()
        {
            var urlBase =
                string.Format(
                    "https://raw.githubusercontent.com/Wiciaki/Releases/master/{0}/Properties/AssemblyInfo.cs",
                    assemblyName);
            webRequest = WebRequest.Create(urlBase);
            webRequest.BeginGetResponse(FinishWebRequest, null);
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            var webResponse = webRequest.EndGetResponse(result);
            // ReSharper disable once AssignNullToNotNullAttribute
            string body = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            if (OnGetVersionCompleted == null)
            {
                return;
            }
            versionCompletedArgs.LastAssemblyVersion = GetVersionFromAssemblyInfo(body);
            OnGetVersionCompleted(versionCompletedArgs);
        }

        private static string GetVersionFromAssemblyInfo(string body)
        {
            string currentVersion = body.Remove(0, body.LastIndexOf("AssemblyVersion", StringComparison.Ordinal));
            currentVersion = currentVersion.Substring(currentVersion.IndexOf("\"", StringComparison.Ordinal) + 1);
            currentVersion = currentVersion.Substring(0, currentVersion.IndexOf("\"", StringComparison.Ordinal));
            return currentVersion;
        }

        // Taken from https://github.com/Hellsing/LeagueSharp/blob/master/Avoid/UpdateChecker.cs
        // Which was c+p'd too, in fact ¯\_(ツ)_/¯

        internal static void Library()
        {
            using (WebClient client = new WebClient())
            {
                new Thread(async () =>
                {
                    try
                    {
                        AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
                        Comms.Print("Assembly Name: " + assemblyName, true);
                        // ReSharper disable once AccessToDisposedClosure
                        string data = await client.DownloadStringTaskAsync("https://raw.github.com/Wiciaki/Releases/master/SparkTech/Properties/AssemblyInfo.cs");
                        var version = Version.Parse(new Regex("AssemblyFileVersion\\((\"(.+?)\")\\)").Match(data).Groups[1].Value.Replace("\"", ""));
                        if (version == assemblyName.Version)
                        {
                            if (!Settings.SkipNoUpdate)
                            {
                                Comms.Print("You are using the latest version of [ST] library.");
                            }
                        }
                        else if (version != assemblyName.Version)
                        {
                            string message = string.Format(
                                "A new spaghetti sauce is available: {0} => {1}",
                                assemblyName.Version,
                                version);
                            Comms.Print(message);
                        }
                        else
                        {
                            Comms.Print("Checking for an update FAILED! (else)", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Comms.Print("Checking for an update FAILED! Exception: " + ex, true);
                    }
                }
                    ).Start();
            }
        }
    }

    public class OnGetVersionCompletedArgs : EventArgs
    {
        public string LastAssemblyVersion;
    }
}