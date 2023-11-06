using LiteNetLib.Utils;
using LiteNetLib;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

namespace Chat
{
    public class ChatServer : MonoBehaviour, INetEventListener
    {
        private NetManager _netServer;
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

        void Start()
        {
            _netPacketProcessor.SubscribeReusable<PlayerMessagePacket, NetPeer>(PlayerMessagePacketReceived);

            _netServer = new NetManager(this);
            _netServer.Start(6000);
        }

        void Update()
        {
            _netServer.PollEvents();
        }

        void OnDestroy()
        {
            if (_netServer != null)
                _netServer.Stop();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
        }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }


        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            _netPacketProcessor.ReadAllPackets(reader, peer);
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var peer = request.AcceptIfKey("chat_app");
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        }

        //Sender

        //Receiver
        private void PlayerMessagePacketReceived(PlayerMessagePacket packet, NetPeer peer)
        {
            var packet1 = new ServerMessagePacket()
            {
                Sender = "Player" + peer.Id,
                Content = packet.Content,
            };
            _netServer.SendToAll(_netPacketProcessor.Write(packet1), DeliveryMethod.ReliableSequenced);
        }
    }
}