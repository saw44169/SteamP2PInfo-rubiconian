using System;
using System.Linq;
using Steamworks;

namespace SteamP2PInfo
{
    /// <summary>
    /// Represents a Steam P2P peer connected using the ISteamNetworking API.
    /// </summary>
    class SteamPeerOldAPI : SteamPeerBase
    {
        /// <summary>
        /// Combination of IP/port. Used query ping for old API connections using the ETW ping monitor.
        /// </summary>
        private ulong mNetIdentity;

        /// <summary>
        /// Object providing information on the steam P2P connection.
        /// </summary>
        private P2PSessionState_t mSessionState;

        public override bool IsOldAPI { get { return true; } }

        public override string ConnectionTypeName => "SteamNetworking";

        /// <summary>
        /// Ping to peer in milliseconds.
        /// </summary>
        public override ConnectionStatistics Ping => this._ping;

        public override ConnectionStatistics ConnectionQuality => this._connectionQuality;

        public override ConnectionStatistics ConnectionQualityRemote => this._connectionQualityRemote;

        public override string UsingRelay { get { return mSessionState.m_bUsingRelay.ToString(); } }

        private ConnectionStatistics _ping;
        private ConnectionStatistics _connectionQuality;
        private ConnectionStatistics _connectionQualityRemote;

        public SteamPeerOldAPI(CSteamID steamId) : base(steamId)
        {
            mSessionState = new P2PSessionState_t();
            _ping = new ConnectionStatistics();
            _connectionQuality = new ConnectionStatistics();
            _connectionQualityRemote = new ConnectionStatistics();
            _connectionQualityRemote.AppendValue(-1);
        }

        public override void Dispose()
        {
            ETWPingMonitor.Unregister(mNetIdentity);
        }

        public override bool UpdatePeerInfo()
        {
            if (!SteamNetworking.GetP2PSessionState(SteamID, out P2PSessionState_t session) || !IsSessionStateOK(session))
                return false;

            bool endpointChanged = mSessionState.m_nRemoteIP != session.m_nRemoteIP || mSessionState.m_nRemotePort != session.m_nRemotePort;
            mSessionState = session;

            if (endpointChanged)
            {
                ETWPingMonitor.Unregister(mNetIdentity);

                byte[] ipBytes = BitConverter.GetBytes(mSessionState.m_nRemoteIP).Reverse().ToArray();
                mNetIdentity = ((ulong)mSessionState.m_nRemotePort << 32) | BitConverter.ToUInt32(ipBytes, 0);
                ETWPingMonitor.Register(mNetIdentity);
            }

            this._ping.AppendValue(ETWPingMonitor.GetPing(mNetIdentity));
            this._connectionQuality.AppendValue(1d / ((0.01d * ETWPingMonitor.GetJitter(mNetIdentity)) + 1d));

            return true;
        }

        public static bool IsSessionStateOK(P2PSessionState_t session)
        {
            return session.m_eP2PSessionError == 0 && (session.m_bConnecting != 0 || session.m_bConnectionActive != 0);
        }
    }
}
