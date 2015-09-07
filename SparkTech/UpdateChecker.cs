using System;
using System.IO;
using System.Net;
using System.Diagnostics.CodeAnalysis;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public class UpdateChecker
    {
        public event onGetVersionCompleted OnGetVersionCompleted;

        // ReSharper disable once InconsistentNaming
        public delegate void onGetVersionCompleted(OnGetVersionCompletedArgs args);

        private WebRequest webRequest;
        private readonly string assemblyName;
        private readonly OnGetVersionCompletedArgs versionCompletedArgs;

        public UpdateChecker(string pAssemblyName)
        {
            assemblyName = pAssemblyName;
            versionCompletedArgs = new OnGetVersionCompletedArgs();
        }

        public void GetLastVersionAsync()
        {
            string urlBase =
                string.Format(
                    @"https://raw.githubusercontent.com/Wiciaki/Releases/master/{0}/Properties/AssemblyInfo.cs",
                    assemblyName);
            webRequest = WebRequest.Create(urlBase);
            webRequest.BeginGetResponse(FinishWebRequest, null);
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            WebResponse webResponse = webRequest.EndGetResponse(result);
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
    }

    public class OnGetVersionCompletedArgs : EventArgs
    {
        public string LastAssemblyVersion;
    }
}