using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class TcpConnectedClient
{
    private TcpClient client;
    private Queue<byte[]> dataReceived = new Queue<byte[]>();
    private byte[] readBuffer = new byte[5000];
    private object readHandler = new object();


    private NetworkStream NetworkStream => client?.GetStream();


    public TcpConnectedClient (TcpClient client)
    {
        this.client = client;

        if(TcpManager.Instance.IsServer)
        {
            NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }
    }

    private void OnRead (IAsyncResult asyncResult)
    {
        int bytesRead = NetworkStream.EndRead(asyncResult);

        if (bytesRead == 0)
        {
            TcpManager.Instance.DisconnectClient(this);
            return;
        }

        byte[] data = new byte[bytesRead];
        Array.Copy(readBuffer, data, bytesRead);

        lock (readHandler)
        {
            dataReceived.Enqueue(data);
        }

        Array.Clear(readBuffer, 0, readBuffer.Length);
        NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    public void SendData(byte[] data)
    {
        NetworkStream.Write(data, 0, data.Length);
    }

    public void FlushRecievedData()
    {
        lock (readHandler)
        {
            while (dataReceived.Count > 0)
            {
                byte[] data = dataReceived.Dequeue();
                TcpManager.Instance.ReceiveData(data);
            }
        }
    }

    public void OnEndConnection (IAsyncResult asyncResult)
    {
        client.EndConnect(asyncResult);
        NetworkStream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    public void CloseClient()
    {
        client.Close();
    }
}
