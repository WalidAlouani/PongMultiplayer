using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameServer : MonoBehaviour, INetEventListener
{
    private NetManager _netServer;
    private readonly NetPacketProcessor _netPacketProcessor = new NetPacketProcessor();

    void Start()
    {
        _netServer = new NetManager(this);
        _netServer.Start(5000);
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
        request.AcceptIfKey("pong_app");
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Debug.Log("[SERVER] We have new peer " + peer.EndPoint);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Debug.Log("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
    }
}
