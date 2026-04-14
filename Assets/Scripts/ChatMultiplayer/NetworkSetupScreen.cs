using UnityEngine;
using System.Net;
using UnityEngine.UI;
using TMPro;
using System;

public class NetworkSetupScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField serverIpField;
    [SerializeField] private TMP_InputField serverPortField;
    [SerializeField] private TMP_InputField clientNameField;

    public string username;

    [SerializeField] private Button startServerButton;
    [SerializeField] private Button connectToServerButton;
    [SerializeField] private GameObject chatScreen; 
    [SerializeField] private GameObject setupScreen;


    void Awake()
    {
        chatScreen.SetActive(false);
        startServerButton.onClick.AddListener(OnStartServer);
        connectToServerButton.onClick.AddListener(OnConnectToServer);
    }

    private void OnDestroy()
    {
        startServerButton.onClick.RemoveListener(OnStartServer);
        connectToServerButton.onClick.RemoveListener(OnConnectToServer);
    }

    private void OnStartServer()
    {
        int port = Convert.ToInt32(serverPortField.text);
        TcpManager.Instance.StartServer(port);

        username = clientNameField.text;

        MoveToChatScreen();
    }

    private void OnConnectToServer()
    {
        IPAddress ipAddress = IPAddress.Parse(serverIpField.text);
        int port = Convert.ToInt32(serverPortField.text);

        username = clientNameField.text;

        TcpManager.Instance.StartClient(ipAddress, port);
        TcpManager.Instance.OnClientConnected += MoveToChatScreen;
    }

    private void MoveToChatScreen()
    {
        setupScreen.SetActive(false);
        chatScreen.SetActive(true);
    }

}
