using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer
{
    public NetPeer NetPeer;
    public MoveRacket View;

    private GameClient gameClient;
    public string axis = "Vertical2";

    public LocalPlayer(GameClient gameClient)
    {
        this.gameClient = gameClient;
    }

    internal void UpdateLogic()
    {
        gameClient.SendInputs(Input.GetAxisRaw(axis));
    }
}
