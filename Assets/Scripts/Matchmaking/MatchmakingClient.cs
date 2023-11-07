using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

namespace Matchmaking
{
    public class MatchmakingClient : MonoBehaviour, INetEventListener
    {
        private NetManager _netClient;
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
        private NetPeer _serverPeer;

        public bool IsConnected => _serverPeer != null && _serverPeer.ConnectionState == ConnectionState.Connected;

        void Start()
        {
            _netClient = new NetManager(this);
            _netClient.Start();
        }

        public void Connect()
        {
            if (!IsConnected)
                _netClient.Connect("localhost", 7000, "matchmaking_app");
            else
                _netClient.DisconnectAll();
        }

        void Update()
        {
            _netClient.PollEvents();
        }

        void OnDestroy()
        {
            if (_netClient != null)
                _netClient.Stop();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _netPacketProcessor.ReadAllPackets(reader, peer);
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            request.Reject();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[CLIENT] We connected to " + peer.EndPoint);
            _serverPeer = peer;
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
            _serverPeer = null;
        }

        ///Senders

        ///Receivers


    }
}
