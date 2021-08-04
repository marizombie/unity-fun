using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client
{
    public class Client
    {
        private int remotePort = ServerData.Port;
        private string serverAddress = ServerData.IpAddress;
        private readonly UdpClient udpClient;
        private IPEndPoint endPoint;

        public ConcurrentQueue<byte[]> MessageDataStorage { get; set; }
    
        public Client()
        {
            udpClient = new UdpClient();
            udpClient.Connect(serverAddress, remotePort);

            MessageDataStorage = new ConcurrentQueue<byte[]>();
        }

        public void SendMessage(byte[] messageBytes)
        {
            try
            {
                udpClient.Send(messageBytes, messageBytes.Length);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private void Receive()
        {
            while (true)
            {
                var data = udpClient.Receive(ref endPoint);
                MessageDataStorage.Enqueue(data);
            }
        }

        public async void ReceiveMessage()
        {
            try
            {
                await Task.Run(Receive);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
