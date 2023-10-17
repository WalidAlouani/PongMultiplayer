using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClientBall : MonoBehaviour
{
    private Vector2 _newPos;
    private Vector2 _oldPos;


    internal void ReceiveState(Vector2 position)
    {
        _oldPos = _newPos;
        _newPos = position;
    }

    internal void UpdateLogic(float lerpTime)
    {
        var pos = transform.position;
        pos.x = Mathf.Lerp(_oldPos.x, _newPos.x, lerpTime);
        pos.y = Mathf.Lerp(_oldPos.y, _newPos.y, lerpTime);
        transform.position = pos;
    }

}
