using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using SteamP2PInfo.Config;
using Steamworks;

namespace SteamP2PInfo
{
    /// <summary>
    /// Manage a list of active Steam P2P peers. The peers must be in a steam lobby with the current user to be detected.
    /// They will automatically be removed from the list if no packet was sent/recieved for a set amount of time.
    /// </summary>
    static class SteamPeerManager
    {
        private static FileStream fs;
        private static StreamReader sr;
        private static FileSystemWatcher fsWatcher;
        private static bool mustReopenLog = true;
        private static long? lastPosInLog = null;
        private static Stopwatch sw = new Stopwatch();

        private static readonly Regex STEAMID3_REGEX = new Regex(@"\[U:1:(?<id>\d+)\]", RegexOptions.Compiled);
        private const long STEAMID64_BASE = 0x0110_0001_0000_0000;

        private const long PEER_TIMEOUT_MS = 5000;

        private static readonly Func<CSteamID, SteamPeerBase>[] PEER_FACTORIES =
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SteamPeerBase)))
                .Select(t => new Func<CSteamID, SteamPeerBase>((CSteamID sid) => Activator.CreateInstance(t, sid) as SteamPeerBase))
                .ToArray();


        /// <summary>
        /// List of peers mapped by Steam ID.
        /// </summary>
        private static Dictionary<CSteamID, SteamPeerInfo> mPeers = new Dictionary<CSteamID, SteamPeerInfo>();

        public static List<SessionHistoryItem> SessionHistory { get; private set; } = new List<SessionHistoryItem>();

        public static void Init()
        {
            fsWatcher = new FileSystemWatcher(Path.GetDirectoryName(AppConfig.Instance.SteamLogPath));
            fsWatcher.Filter = Path.GetFileName(AppConfig.Instance.SteamLogPath);
            fsWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            fsWatcher.Changed += (e, s) => mustReopenLog = true;
            fsWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// extract SteamID from log
        /// </summary>
        /// <param name="str">string read from log</param>
        /// <returns></returns>
        private static CSteamID ExtractUser(string str)
        {
            Match m = STEAMID3_REGEX.Match(str);
            if (m.Success)
            {
                return new CSteamID(ulong.Parse(m.Groups["id"].Value) + STEAMID64_BASE);
            }
            else
            {
                return new CSteamID(0);
            }
        }

        /// <summary>
        /// create new steam peer instance
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private static SteamPeerBase GetPeer(CSteamID player)
        {
            SteamPeerBase peer = null;
            foreach (var factory in PEER_FACTORIES)
            {
                try
                {
                    peer = factory(player);
                    if (peer.UpdatePeerInfo())
                    {
                        Logger.WriteLine($"[PEER CONNECT] \"{peer.Name}\" (https://steamcommunity.com/profiles/{(ulong)peer.SteamID}) has connected via {peer.ConnectionTypeName}");
                        if (GameConfig.Current.SetPlayedWith)
                            SteamFriends.SetPlayedWith(player);

                        return peer;
                    }
                }
                catch (Exception)
                {
                    peer?.Dispose();
                }
            }
            return null;
        }

        public async static void UpdatePeerList()
        {
            // Make sure we're constantly writing to the IPC log to force Steam to eventually flush
            // This call was chosen because it's not something a game will call often
            // Thus we avoid blowing up the IPC log with dummy calls
            SteamFriends.SendClanChatMessage(new CSteamID(0), "");

            if (mustReopenLog)
            {
                sr?.Dispose();
                fs?.Close();
                fs?.Dispose();

                try
                {
                    fs = new FileStream(AppConfig.Instance.SteamLogPath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
                    sr = new StreamReader(fs);
                    // If the file had to be reopened, read from the last position we were at before
                    if (lastPosInLog is null)
                        fs.Seek(0, SeekOrigin.End);
                    else
                        fs.Seek((long)lastPosInLog, SeekOrigin.Begin);
                    mustReopenLog = false;
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("Steam IPC log file directory does not exist", "Directory Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            while (!mustReopenLog)
            {
                string line = await sr.ReadLineAsync();

                // ログの終端に到達したら終了
                if (line == null)
                {
                    lastPosInLog = fs.Position;
                    break;
                }

                // 対象のプロセスに関するログでなければスキップ
                if (!line.Contains(GameConfig.Current.ProcessName))
                    continue;

                bool begin;
                if (line.Contains("BeginAuthSession"))
                {
                    begin = true;
                }
                else if (line.Contains("EndAuthSession"))
                {
                    begin = false;
                }
                else if (line.Contains("LeaveLobby"))
                {
                    foreach (var sid in mPeers.Keys)
                    {
                        LogDisconnect(mPeers[sid].peer, sid, "Player left Steam lobby");
                    }
                    mPeers.Clear();
                    continue;
                }
                else continue;

                CSteamID steamID = ExtractUser(line);

                if (steamID.m_SteamID != 0)
                {
                    if (steamID.BIndividualAccount())
                    {
                        if (begin)
                        {
                            if (!mPeers.ContainsKey(steamID))
                            {
                                var newPeerInfo = new SteamPeerInfo(GetPeer(steamID));
                                if (newPeerInfo.peer is null)
                                {
                                    Logger.WriteLine($"[PEER CONNECT] Player \"{steamID}\" was detected, but we don't have a P2P connection to them yet");
                                    newPeerInfo.lastDisconnectTimeMS = sw.ElapsedMilliseconds;
                                }
                                mPeers.Add(steamID, newPeerInfo);
                            }
                        }
                        else
                        {
                            // peer just disconnected
                            if (mPeers.TryGetValue(steamID, out SteamPeerInfo pInfo))
                            {
                                RemovePeer(steamID);
                                LogDisconnect(pInfo.peer, steamID, "Auth session with peer ended");
                            }
                        }
                    }
                    else
                    {
                        Logger.WriteLine($"[PARSE ERROR] \"{steamID}\" was not a valid steam user");
                    }
                }
            }
            UpdateAllPeerInfo();
            CleanUpOldPeers();
        }

        /// <summary>
        /// log disconnected peer and reason
        /// </summary>
        /// <param name="peer"></param>
        /// <param name="sid"></param>
        /// <param name="reason"></param>
        private static void LogDisconnect(SteamPeerBase peer, CSteamID sid, string reason)
        {
            if (peer is null)
            {
                Logger.WriteLine($"[PEER DISCONNECT] (https://steamcommunity.com/profiles/{(ulong)sid}): {reason}");
            }
            else
            {
                Logger.WriteLine($"[PEER DISCONNECT] \"{peer.Name}\" (https://steamcommunity.com/profiles/{(ulong)sid}): {reason}");
            }
        }

        private static void UpdateAllPeerInfo()
        {
            foreach (var item in mPeers)
            {
                SteamPeerInfo pInfo = item.Value as SteamPeerInfo;
                pInfo.peer?.UpdatePeerInfo();
            }
        }
        private static void CleanUpOldPeers()
        {
            // clean up old peers.
            foreach (var sid in mPeers.Keys.ToArray())
            {
                var pInfo = mPeers[sid];
                bool isP2PConnected = false;
                if (pInfo.isConnected && !isP2PConnected)
                    pInfo.lastDisconnectTimeMS = sw.ElapsedMilliseconds;
                pInfo.isConnected = isP2PConnected;

                if (!isP2PConnected && sw.ElapsedMilliseconds - pInfo.lastDisconnectTimeMS > PEER_TIMEOUT_MS)
                {
                    RemovePeer(sid);
                    LogDisconnect(pInfo.peer, sid, pInfo.peer is null ? "P2P connection was not established" : "Peer disconnected from P2P session");
                }
            }
        }

        private static bool RemovePeer(CSteamID sid)
        {
            SteamPeerInfo pInfo;
            bool exists = mPeers.TryGetValue(sid, out pInfo);
            if (!exists) { return false; }
            SessionHistory.Add(new SessionHistoryItem(pInfo.peer));
            mPeers.Remove(sid);

            return true;
        }

        public static IEnumerable<SteamPeerBase> GetPeers()
        {
            return mPeers.Values.Where(info => info.peer != null).Select(info => info.peer);
        }

        public static void exportHistory()
        {
            string dirPath = GameConfig.Current.HistoryOutputDir;
            if (!Directory.Exists(dirPath))
            {
                Logger.WriteLine("[ERROR] exportHistory failed : specified output directory does not exists");
                return;
            }

            string eol = Environment.NewLine;
            string csvStr = "Start, Steam Name, Steam ID, Relay, Ping Min, Ping Max, Ping Avg, Ping Stdev, CQ Min, CQ Max, CQ Avg, CQ Stdev, CQR Min, CQR Max, CQR Avg, CQR Stdev, Type" + eol;

            foreach (SessionHistoryItem item in SessionHistory)
            {
                string startedAtStr = item.StartedAt.ToString();
                string misc = $"{startedAtStr}, {item.SteamName}, {item.SteamIDStr}, {item.UsingRelay}, ";
                var ping = item.Ping;
                string pingCols = $"{ping.Min}, {ping.Max}, {ping.Avg}, {ping.Stdev}, ";
                var cq = item.ConnectionQuality;
                string cqCols = $"{cq.Min}, {cq.Max}, {cq.Avg}, {cq.Stdev}, ";
                var cqr = item.ConnectionQualityRemote;
                string cqrCols = $"{cqr.Min}, {cqr.Max}, {cqr.Avg}, {cqr.Stdev}, ";
                csvStr += $"{misc}{pingCols}{cqCols}{cqrCols}{item.ConnectionTypeName}{eol}";
            }

            string dateStr = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            string fileName = $"{GameConfig.Current.ProcessName}_{dateStr}.csv";
            string filePath = Path.Combine(dirPath, fileName);

            File.WriteAllText(filePath, csvStr);
            return;

        }
    }
}
