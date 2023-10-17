using LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayer
{
    //public NetPeer NetPeer;
    public GameObject View;
    private float _newPosY;
    private float _oldPosY;

    internal void UpdateLogic(float _lerpTime)
    {
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
