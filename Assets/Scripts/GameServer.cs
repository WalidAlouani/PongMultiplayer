using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameServer : MonoBehaviour, INetEventListener
{
    private NetManager _netServer;
    private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

    private ServerPlayer _player1 = new ServerPlayer();
    private ServerPlayer _player2 = new ServerPlayer();

    [SerializeField]
    private Ball _ball;
    [SerializeField]
    private MoveRacket _racketLeft;
    [SerializeField]
    private MoveRacket _racketRight;

    void Start()
    {
        _netPacketProcessor.SubscribeReusable<PlayerInputsPacket, NetPeer>(PlayerInputsPacketReceived);

        _netServer = new NetManager(this);
        _netServer.Start(5000);

        _player1.View = _racketLeft;
        _player2.View = _racketRight;
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

    void FixedUpdate()
    {
        _player1.UpdateLogic();
        _player2.UpdateLogic();
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
        if (_player1.NetPeer == null)
            _player1.NetPeer = request.AcceptIfKey("pong_app");
        else if (_player2.NetPeer == null)
            _player2.NetPeer = request.AcceptIfKey("pong_app");
        else
            request.Reject();
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);

        if (_player1.NetPeer != null && _player2.NetPeer != null)
            _ball.Init();
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
    }

    //Sender





    //Receiver
    private void PlayerInputsPacketReceived(PlayerInputsPacket packet, NetPeer peer)
    {
        if (_player1.NetPeer == peer)
        {
            _player1.ReceiveInputs(packet.Movement);
        }
        else if (_player2.NetPeer == peer)
        {
            _player2.ReceiveInputs(packet.Movement);
        }
    }
}
