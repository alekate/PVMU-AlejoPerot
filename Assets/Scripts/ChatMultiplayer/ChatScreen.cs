using UnityEngine;
using System.Net;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

public class ChatScreen : MonoBehaviour
{
    [SerializeField] private RectTransform verticalChatRect;
    [SerializeField] private ScrollRect chatScroll;
    [SerializeField] private GameObject chatPrefab;
    [SerializeField] private TMP_InputField messageInputField;
    [SerializeField] private Button sendButton;

    private NetworkSetupScreen networkSetupScreen;

    [SerializeField] private Transform content;
    [SerializeField] private ChatMessageItem messagePrefab;

    [Serializable]
    public class ChatMessage
    {
        public string username;
        public string message;
    }

    void Start()
    {
        chatPrefab.SetActive(false);
        
        TcpManager.Instance.OnDataReceived += OnReceiveData;
        sendButton.onClick.AddListener(OnSendMessage);

        networkSetupScreen = FindFirstObjectByType<NetworkSetupScreen>();
    }

    private void OnDestroy()
    {
        TcpManager.Instance.OnDataReceived -= OnReceiveData;
        sendButton.onClick.RemoveListener(OnSendMessage);
    }

    private void Update()
    {
        AdjustChatSize();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnSendMessage();
        }
    }

    private void AdjustChatSize()
    {
        bool isVertical = Screen.height > Screen.width;

        if (isVertical)
        {
            verticalChatRect.sizeDelta = new Vector2(verticalChatRect.sizeDelta.x, 1812);
        }
        else
        {
            verticalChatRect.sizeDelta = new Vector2(verticalChatRect.sizeDelta.x, 678);
        }
    }

    private void UpdateScroll()
    {
        chatScroll.verticalNormalizedPosition = 0f;
    }


    /*private void OnSendMessage()
    {
        if (string.IsNullOrEmpty(messageInputField.text))
        {
            return;
        }

        byte[] dataMessege = Encoding.UTF8.GetBytes(messageInputField.text);
        byte[] dataUsername = Encoding.UTF8.GetBytes(networkSetupScreen.username);

        if (TcpManager.Instance.IsServer)
        {
            chatText.text += messageInputField.text + Environment.NewLine;
            UpdateScroll();
            TcpManager.Instance.BroadcastData(dataMessege, dataUsername);
        }
        else
        {
            TcpManager.Instance.SendDataToServer(dataMessege, dataUsername);
        }

        messageInputField.text = string.Empty;
    }*/

    private void OnSendMessage()
    {
        if (string.IsNullOrEmpty(messageInputField.text))
        { 
            return;
        }

        ChatMessage msg = new ChatMessage
        {
            username = networkSetupScreen.username,
            message = messageInputField.text
        };

        string json = JsonUtility.ToJson(msg) + "\n";
        byte[] data = Encoding.UTF8.GetBytes(json);

        if (TcpManager.Instance.IsServer)
        {
            bool isMine = msg.username == networkSetupScreen.username;

            ChatMessageItem item = Instantiate(messagePrefab, content);
            item.Setup(msg.username, msg.message, isMine); UpdateScroll();
            TcpManager.Instance.BroadcastData(data);
        }
        else
        {
            TcpManager.Instance.SendDataToServer(data);
        }

        messageInputField.text = string.Empty;
    }

    /*private void OnReceiveData(byte[] dataMessege, byte[] dataUsername)
   {
       if (TcpManager.Instance.IsServer)
       {
           TcpManager.Instance.BroadcastData(dataMessege, dataUsername);
       }

       chatUsername.text += Encoding.UTF8.GetString(dataUsername, 0, dataUsername.Length) + Environment.NewLine;
       chatText.text += Encoding.UTF8.GetString(dataMessege, 0, dataMessege.Length) + Environment.NewLine;
       UpdateScroll();
   }*/
    private void OnReceiveData(byte[] data)
    {
        if (TcpManager.Instance.IsServer)
        {
            TcpManager.Instance.BroadcastData(data);
        }

        string json = Encoding.UTF8.GetString(data);
        string[] messages = json.Split('\n');

        foreach (var message in messages)
        {
            ChatMessage msg = JsonUtility.FromJson<ChatMessage>(message);
            bool isMine = msg.username == networkSetupScreen.username;

            ChatMessageItem item = Instantiate(messagePrefab, content);
            item.Setup(msg.username, msg.message, isMine);
        }

        UpdateScroll();
    }
}
   
