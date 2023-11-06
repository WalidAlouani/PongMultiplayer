using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Chat
{
    public class UI_Manager : MonoBehaviour
    {
        [SerializeField] private ChatClient chatClient;
        [SerializeField] private Button sendButton;
        [SerializeField] private InputField inputField;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject messagePrefab;

        private void Awake()
        {
            sendButton.onClick.AddListener(SendMessage);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Return))
                SendMessage();
        }

        public void SendMessage()
        {
            var content = inputField.text;
            if (string.IsNullOrEmpty(content))
                return;

            inputField.text = string.Empty;
            inputField.ActivateInputField();

            chatClient.SendChatMessage(content);
        }

        public void ReceiveMessage(string sender, string content)
        {
            var message = Instantiate(messagePrefab, scrollRect.content);
            message.GetComponent<Text>().text = "<color=lime><b>[" + sender + "]</b></color> : " + content;
            Task.Delay(100);

            StartCoroutine(UpdateScrollBar());
        }

        IEnumerator UpdateScrollBar()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            scrollRect.verticalScrollbar.value = 0;
        }
    }
}
