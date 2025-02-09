using Steamworks;

namespace SteamP2PInfo
{
    /// <summary>
    /// Represents a Steam P2P peer connected using the ISteamNetworkingMessages API.
    /// </summary>
    class SteamPeerNewAPI : SteamPeerBase
    {
        /// <summary>
        /// Object providing information on the steam P2P connection.
        /// </summary>
        private SteamNetConnectionInfo_t mConnInfo;

        /// <summary>
        /// Object providing realtime information on the steam P2P connection (namely ping)
        /// </summary>
        private SteamNetConnectionRealTimeStatus_t mRealTimeStatus;

        public override bool IsOldAPI { get { return false; } }

        public override string ConnectionTypeName => "SteamNetworkingSockets";

        public override ConnectionStatistics Ping => this._ping;

        public override ConnectionStatistics ConnectionQuality => this._connectionQuality;

        public override ConnectionStatistics ConnectionQualityRemote => this._connectionQualityRemote;

        // What relay are we using to communicate with the remote host?
        // (0 if not applicable.)
        public override string UsingRelay { get { return mConnInfo.m_idPOPRelay.ToString(); } }

        private ConnectionStatistics _ping;
        private ConnectionStatistics _connectionQuality;
        private ConnectionStatistics _connectionQualityRemote;

        public SteamPeerNewAPI(CSteamID steamId) : base(steamId)
        {
            mConnInfo = new SteamNetConnectionInfo_t();
            mRealTimeStatus = new SteamNetConnectionRealTimeStatus_t();
            _ping = new ConnectionStatistics();
            _connectionQuality = new ConnectionStatistics();
            _connectionQualityRemote = new ConnectionStatistics();
        }

        public override bool UpdatePeerInfo()
        {
            SteamNetworkingIdentity networkingIdentity = new SteamNetworkingIdentity();
            networkingIdentity.SetSteamID(SteamID);

            var connState = SteamNetworkingMessages.GetSessionConnectionInfo(ref networkingIdentity, out mConnInfo, out mRealTimeStatus);
            this._ping.AppendValue(mRealTimeStatus.m_nPing);
            this._connectionQuality.AppendValue(mRealTimeStatus.m_flConnectionQualityLocal);
            this._connectionQualityRemote.AppendValue(mRealTimeStatus.m_flConnectionQualityRemote);
            return IsConnStateOK(connState);
        }

        public static bool IsConnStateOK(ESteamNetworkingConnectionState connState)
        {
            return connState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting ||
                     connState == ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected;
        }
    }
}
