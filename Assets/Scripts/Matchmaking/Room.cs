using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading.Tasks;

public class Room
{
    private NetPeer _peer1;
    private NetPeer _peer2;
    private int _port;
    private string _password;
    private GSProcess _process;

    public Room(NetPeer peer1, NetPeer peer2, int port)
    {
        _peer1 = peer1;
        _peer2 = peer2;
        _port = port;
        _password = System.Guid.NewGuid().ToString();
        _process = new GSProcess(_port, _password);

        RoomIsReady();
    }

    public async void RoomIsReady() 
    {
        await Task.Delay(5000);

        NetDataWriter writer = new NetDataWriter();
        writer.Put(_port);
        writer.Put(_password);
        _peer1.Disconnect(writer);
        _peer2.Disconnect(writer);
    }
}
