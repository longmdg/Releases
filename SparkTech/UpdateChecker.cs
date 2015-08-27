using System;
using System.IO;
using System.Net;

namespace SparkTech
{
    public class UpdateChecker
    {
        public delegate void OnGetVersionCompleted(OnGetVersionCompletedArgs args);

        // ReSharper disable once InconsistentNaming
        public event OnGetVersionCompleted onGetVersionCompleted;

        private WebRequest _webRequest;
        private readonly string _assemblyName;
        private readonly OnGetVersionCompletedArgs _versionCompletedArgs;


        public UpdateChecker(string pAssemblyName)
        {
            _assemblyName = pAssemblyName;

            _versionCompletedArgs = new OnGetVersionCompletedArgs();
        }

        public void GetLastVersionAsync()
        {
            var urlBase =
                string.Format(
                    @"https://raw.githubusercontent.com/Wiciaki/Releases/master/{0}/Properties/AssemblyInfo.cs",
                    _assemblyName);
            _webRequest = WebRequest.Create(urlBase);
            _webRequest.BeginGetResponse(FinishWebRequest, null);
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            var webResponse = _webRequest.EndGetResponse(result);
            // ReSharper disable once AssignNullToNotNullAttribute
            var body = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();

            if (onGetVersionCompleted == null)
                return;
            _versionCompletedArgs.LastAssemblyVersion = GetVersionFromAssemblyInfo(body);
            onGetVersionCompleted(_versionCompletedArgs);
        }

        private static string GetVersionFromAssemblyInfo(string body)
        {
            var currentVersion = body.Remove(0, body.LastIndexOf("AssemblyVersion", StringComparison.Ordinal));
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
