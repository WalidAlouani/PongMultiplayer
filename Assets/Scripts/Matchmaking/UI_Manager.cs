using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Matchmaking
{
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] private Button _buttonConnect;
        [SerializeField] private Text _textConnect;
        [SerializeField] private MatchmakingClient _client;

        void Awake()
        {
            _buttonConnect.onClick.AddListener(_client.Connect);
        }

        void Update()
        {
            _textConnect.text = _client.IsConnected ? "Disconnect" : "Connect";
        }
    }
}
