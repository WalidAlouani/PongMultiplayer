using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Playables;

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
        _netPacketProcessor.RegisterNestedType<PlayerState>();
        _netPacketProcessor.RegisterNestedType<LNLVector2>();
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

        SendGameState(_player1.GetState(), _player2.GetState(), _ball.GetState());
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
        {
            var peer = request.AcceptIfKey("pong_app");
            _player1.NetPeer = peer;
            var packet = new ClientSidePacket()
            {
                LeftSide = true,
            };
            peer.Send(_netPacketProcessor.Write(packet), DeliveryMethod.ReliableOrdered);
        }
        else if (_player2.NetPeer == null)
        {
            var peer = request.AcceptIfKey("pong_app");
            _player2.NetPeer = peer;
            var packet = new ClientSidePacket()
            {
                LeftSide = false,
            };
            peer.Send(_netPacketProcessor.Write(packet), DeliveryMethod.ReliableOrdered);
        }
        else
        {
            request.Reject();
        }
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

        if (_player1.NetPeer == peer)
            _player1.Disconnect();
        else if (_player2.NetPeer == peer)
            _player2.Disconnect();

        if (_player1.NetPeer == null && _player2.NetPeer == null)
            _ball.Stop();
    }

    //Sender
    internal void SendGameState(PlayerState player1, PlayerState player2, LNLVector2 ballPosition)
    {
        var packet = new GameStatePacket()
        {
            Player1 = player1,
            Player2 = player2,
            Ball = ballPosition,
        };

        _netServer.SendToAll(_netPacketProcessor.Write(packet), DeliveryMethod.ReliableSequenced);
    }

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
