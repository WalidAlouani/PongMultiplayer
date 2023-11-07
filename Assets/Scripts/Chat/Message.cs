using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Chat
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private Text text;

        public void DisplayMessage(string sender, string content)
        {
            text.text = "<color=lime><b>[" + sender + "]</b></color> : " + content;
        }

        public void DisplayJoinedMessage(string playerName)
        {
            text.text = "<color=green><b>Player [" + playerName + "] has joined the chat.</b></color>";
        }
        
        public void DisplayLeftMessage(string playerName)
        {
            text.text = "<color=red><b>Player [" + playerName + "] has left the chat.</b></color>";
        }
    }
}