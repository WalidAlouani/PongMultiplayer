using LiteNetLib;
using LiteNetLib.Utils;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Matchmaking
{
    public delegate void Notify(bool isConnected);  // delegate

    public class MatchmakingClient : MonoBehaviour, INetEventListener
    {
        private NetManager _netClient;
        private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
        private NetPeer _serverPeer;

        public bool IsConnected => _serverPeer != null && _serverPeer.ConnectionState == ConnectionState.Connected;
        public event Notify OnConnectedChanged; // event
        //public Action<bool> OnConnectedChanged; // event

        void Start()
        {
            _netClient = new NetManager(this);
            _netClient.Start();
        }

        public void Connect()
        {
            if (!IsConnected)
            {
                var writer = new NetDataWriter();
                writer.Put("matchmaking_app");

                //var packet = new JoinPacket()
                //{
                //    Key = "matchmaking_app",
                //};

                //var writer = NetDataWriter.FromBytes(_netPacketProcessor.Write(packet), false);

                _netClient.Connect("localhost", 7000, writer);
            }
            else
            {
                _netClient.DisconnectAll();
            }
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
            OnConnectedChanged?.Invoke(IsConnected);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[CLIENT] We disconnected because " + disconnectInfo.Reason);
            _serverPeer = null;
            OnConnectedChanged?.Invoke(IsConnected);

            //if (disconnectInfo.Reason == DisconnectReason.DisconnectPeerCalled)
            //    if (disconnectInfo.AdditionalData.AvailableBytes > 0)
            //        _netPacketProcessor.ReadPacket(disconnectInfo.AdditionalData);

            if (disconnectInfo.Reason == DisconnectReason.RemoteConnectionClose)
                if (disconnectInfo.AdditionalData.AvailableBytes > 0)
                {
                    if (!disconnectInfo.AdditionalData.TryGetInt(out var port))
                        return;

                    if (!disconnectInfo.AdditionalData.TryGetString(out var password))
                        return;

                    Debug.Log(port + " " + password);
                    GameClient.GSPort = port;
                    GameClient.GSPassword = password;
                    SceneManager.LoadScene("scene_client");
                }
        }

        ///Senders

        ///Receivers


    }
}
