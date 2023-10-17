//C->S
using LiteNetLib.Utils;

class PlayerInputsPacket
{
    public float Movement { get; set; }
}

//S->C
class GameStatePacket
{
    public PlayerState Player1 { get; set; }
    public PlayerState Player2 { get; set; }
    public LNLVector2 Ball { get; set; }
}

struct PlayerState : INetSerializable
{
    public int PlayerId { get; set; }
    public float PositionY { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(PlayerId);
        writer.Put(PositionY);
    }

    public void Deserialize(NetDataReader reader)
    {
        PlayerId = reader.GetInt();
        PositionY = reader.GetFloat();
    }
}

struct LNLVector2 : INetSerializable
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(PositionX);
        writer.Put(PositionY);
    }

    public void Deserialize(NetDataReader reader)
    {
        PositionX = reader.GetFloat();
        PositionY = reader.GetFloat();
    }
}