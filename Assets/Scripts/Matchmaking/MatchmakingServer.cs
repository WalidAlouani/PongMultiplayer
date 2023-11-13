using Chat;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System;

namespace Matchmaking
{
    public class MatchmakingServer : MonoBehaviour, INetEventListener
    {
        private NetManager _netServer;
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

        private List<NetPeer> Peers = new List<NetPeer>();
        private List<Room> Rooms = new List<Room>();

        private int ports = 5000;

        void Start()
        {
            _netPacketProcessor.SubscribeReusable<JoinPacket, NetPeer>(JoinPacketReceived);

            _netServer = new NetManager(this);
            _netServer.Start(7000);
        }

        void Update()
        {
            _netServer.PollEvents();

            ProcessMatchmaking();
        }

        private void ProcessMatchmaking()
        {
            while (Peers.Count >= 2)
            {
                var peer1 = Peers[0];
                Peers.RemoveAt(0);
                var peer2 = Peers[0];
                Peers.RemoveAt(0);

                var room = new Room(peer1, peer2, ++ports);
                Rooms.Add(room);
            }
        }

        void OnDestroy()
        {
            if (_netServer != null)
                _netServer.Stop();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }
        
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _netPacketProcessor.ReadAllPackets(reader, peer);
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            //_netPacketProcessor.ReadPacket(request.Data);

            var reader = request.Data;
            var key = reader.GetString();
            if (key.Equals("matchmaking_app"))
                request.Accept();
            else
                request.Reject();
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
            Peers.Add(peer);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
            Peers.Remove(peer);
        }

        ///Senders

        ///Receivers
        ///
        private void JoinPacketReceived(JoinPacket packet, NetPeer peer)
        {
            Debug.Log(packet.Key);
        }

    }
}
