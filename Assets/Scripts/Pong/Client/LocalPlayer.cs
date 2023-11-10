using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer
{
    //public NetPeer NetPeer;
    public GameObject View;

    private GameClient gameClient;
    public string axis = "Vertical2";

    private float _newPosY;
    private float _oldPosY;

    public LocalPlayer(GameClient gameClient)
    {
        this.gameClient = gameClient;
    }

    internal void UpdateLogic(float _lerpTime)
    {
        gameClient.SendInputs(Input.GetAxisRaw(axis));

        if (View == null)
            return;

        var pos = View.transform.position;
        pos.y = Mathf.Lerp(_oldPosY, _newPosY, _lerpTime);
        View.transform.position = pos;
    }

    internal void ReceiveState(float positionY)
    {
        _oldPosY = _newPosY;
        _newPosY = positionY;
    }
}
