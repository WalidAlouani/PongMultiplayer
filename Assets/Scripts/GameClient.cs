using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameClient : MonoBehaviour, INetEventListener
{
    private NetManager _netClient;
    private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
    private NetPeer _serverPeer;

    private LocalPlayer _playerLocal;
    private RemotePlayer _playerRemote;

    [SerializeField]
    private Ball _ball;
    [SerializeField]
    private MoveRacket _racketLeft;
    [SerializeField]
    private MoveRacket _racketRight;

    void Start()
    {
        _netClient = new NetManager(this);
        _netClient.Start();
        _netClient.Connect("localhost", 5000, "pong_app");

        _playerLocal = new LocalPlayer(this);
        _playerLocal.View = _racketRight;
        _playerRemote = new RemotePlayer();
        _playerRemote.View = _racketLeft;
    }

    void Update()
    {
        _netClient.PollEvents();

        if (_serverPeer == null || _serverPeer.ConnectionState != ConnectionState.Connected)
            return;

        _playerLocal.UpdateLogic();
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

    internal void SendInputs(float movement)
    {
        var packet = new PlayerInputsPacket()
        {
            Movement = movement
        };

        _netPacketProcessor.Send(_serverPeer, packet, DeliveryMethod.ReliableSequenced);
    }
}
