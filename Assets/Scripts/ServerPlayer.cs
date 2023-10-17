using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPlayer
{
    public NetPeer NetPeer;
    public MoveRacket View;

    internal void ReceiveInputs(float movement)
    {
        View.Movement = movement;
    }

    internal void UpdateLogic()
    {
        View.UpdateLogic();
    }

    internal PlayerState GetState()
    {
        return new PlayerState()
        {
            PlayerId = NetPeer == null ? -1 : NetPeer.Id,
            PositionY = View.transform.position.y,
        };
    }

    internal void Disconnect()
    {
        NetPeer = null;
    }
}
