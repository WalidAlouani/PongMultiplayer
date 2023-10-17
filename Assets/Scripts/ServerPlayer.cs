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
}
