using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;

namespace Chat
{
    public class ChatClient : MonoBehaviour, INetEventListener
    {
        [SerializeField] private UI_Manager _uiManager;
        private NetManager _netClient;
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
        private NetPeer _serverPeer;

        void Start()
        {
            _netPacketProcessor.SubscribeReusable<ServerMessagePacket, NetPeer>(ServerMessagePacketReceived);

            _netClient = new NetManager(this);
            _netClient.Start();
            _netClient.Connect("localhost", 6000, "chat_app");
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


        //Senders
        internal void SendChatMessage(string content)
        {
            var packet = new PlayerMessagePacket()
            {
                Content = content,
            };

            _netPacketProcessor.Send(_serverPeer, packet, DeliveryMethod.ReliableOrdered);
        }

        //Receiver
        private void ServerMessagePacketReceived(ServerMessagePacket packet, NetPeer peer)
        {
            _uiManager.ReceiveMessage(packet.Sender, packet.Content);
        }
    }
}
