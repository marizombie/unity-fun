using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Client
    {
        private int remotePort = 8000;
        private string serverAddress = "127.0.0.1";
        private readonly UdpClient udpClient;
        private IPEndPoint endPoint;

        public ConcurrentQueue<byte[]> MessageDataStorage { get; set; }
    
        public Client()
        {
            //endPoint = new IPEndPoint(IPAddress.Parse(serverAddress), remotePort);
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
