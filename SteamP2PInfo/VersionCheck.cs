using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace SteamP2PInfo
{
    static class VersionCheck
    {
        public static readonly string CurrentVersion = "V1.2.0";
        public static readonly string repositoryName = "saw44169/SteamP2PInfo-rubiconian";
        public static JObject LatestRelease { get; private set; }

        public static bool FetchLatest()
        {
            string query = "https://api.github.com/repos/" + repositoryName + "/releases";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(query);
            req.UserAgent = "request";
            HttpWebResponse resp;

            try
            {
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException)
            {
                LatestRelease = null;
                return false;
            }

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    JArray data = JArray.Parse(reader.ReadToEnd());
                    if (data.Count != 0) LatestRelease = (JObject)data.Where(r => !(bool)r["prerelease"]).First();
                    return data.Count != 0;
                }
            }

            LatestRelease = null;
            return false;
        }
    }
}
