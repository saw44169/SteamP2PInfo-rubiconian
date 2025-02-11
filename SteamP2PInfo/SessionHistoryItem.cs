using System;

namespace SteamP2PInfo
{
    internal class SessionHistoryItem
    {
        public DateTime StartedAt { get; private set; }
        public string SteamIDStr { get; private set; }
        public string SteamName { get; private set; }
        public string UsingRelay { get; private set; }
        public ConnectionStatistics Ping { get; private set; }
        public ConnectionStatistics ConnectionQuality { get; private set; }
        public ConnectionStatistics ConnectionQualityRemote { get; private set; }
        public string ConnectionTypeName { get; private set; }

        public SessionHistoryItem(SteamPeerBase peer)
        {
            StartedAt = peer.StartedAt;
            SteamIDStr = peer.SteamID.ToString();
            SteamName = peer.Name;
            UsingRelay = peer.UsingRelay;
            Ping = peer.Ping;
            ConnectionQuality = peer.ConnectionQuality;
            ConnectionQualityRemote = peer.ConnectionQualityRemote;
            ConnectionTypeName = peer.ConnectionTypeName;
        }

        public SessionHistoryItem()
        {
            StartedAt = DateTime.Now;
            SteamIDStr = "1";
            SteamName = "hoge";
            UsingRelay = "0";
            ConnectionTypeName = "peer.ConnectionTypeName";
        }

    }
}
