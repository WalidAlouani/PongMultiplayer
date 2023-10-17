using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Playables;

public class GameClient : MonoBehaviour, INetEventListener
{
    private NetManager _netClient;
    private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();
    private NetPeer _serverPeer;

    private LocalPlayer _playerLocal;
    private RemotePlayer _playerRemote;

    [SerializeField]
    private GameObject _ball;
    [SerializeField]
    private GameObject _racketLeft;
    [SerializeField]
    private GameObject _racketRight;

    void Start()
    {
        _netPacketProcessor.RegisterNestedType<PlayerState>();
        _netPacketProcessor.RegisterNestedType<LNLVector2>();
        _netPacketProcessor.SubscribeReusable<GameStatePacket, NetPeer>(GameStatePacketReceived);

        _netClient = new NetManager(this);
        _netClient.Start();
        _netClient.Connect("localhost", 5000, "pong_app");

        _playerLocal = new LocalPlayer(this);
        _playerRemote = new RemotePlayer();
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

    //Receiver

    private void GameStatePacketReceived(GameStatePacket packet, NetPeer peer)
    {
        if (peer.RemoteId == packet.Player1.PlayerId)
        {
            _playerLocal.View = _racketLeft;
            _playerRemote.View = _racketRight;

            _playerLocal.View.transform.position = new Vector2(_playerLocal.View.transform.position.x, packet.Player1.PositionY);
            _playerRemote.View.transform.position = new Vector2(_playerRemote.View.transform.position.x, packet.Player2.PositionY);
        }
        else if (peer.RemoteId == packet.Player2.PlayerId)
        {
            _playerLocal.View = _racketRight;
            _playerRemote.View = _racketLeft;

            _playerLocal.View.transform.position = new Vector2(_playerLocal.View.transform.position.x, packet.Player2.PositionY);
            _playerRemote.View.transform.position = new Vector2(_playerRemote.View.transform.position.x, packet.Player1.PositionY);
        }

        _ball.transform.position = new Vector2(packet.Ball.PositionX, packet.Ball.PositionY);
    }
}
