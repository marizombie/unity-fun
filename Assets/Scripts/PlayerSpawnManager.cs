using System;
using System.Collections.Generic;
using FlatBuffers;
using Messages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        private float mapDim = 5;
        private Client client;

        public GameObject playerPrefab;
        public Dictionary<uint, GameObject> playersDictionary;

        private void Start()
        {
            playersDictionary = new Dictionary<uint, GameObject>();

            client = new Client();
            client.ReceiveMessage();

            var startingPosition = new Vector3(Random.Range(-mapDim, mapDim), playerPrefab.transform.localScale.y / 2, 0);
            //get player id from server when player logs in?
            var playerId = Random.Range(0, 100);
            SpawnNewPlayer((uint)playerId, "initial player", startingPosition, isLocalPlayer: true);
        }

        private void Update()
        {
            if (client.MessageDataStorage.IsEmpty) return;

            if (!client.MessageDataStorage.TryDequeue(out var results)) return;

            var bytesBuffer = new ByteBuffer(results);
            if (bytesBuffer.Length == 0)
            {
                Debug.Log("bytes buffer is empty");

                return;
            }

            var receivedMessage = Message.GetRootAsMessage(bytesBuffer);

            var playerId = receivedMessage.PlayerId; 
            var serverPosition = new Vector3(receivedMessage.X, receivedMessage.Y, receivedMessage.Z);
            var containsId = playersDictionary.TryGetValue(receivedMessage.PlayerId, out var player);
            if (containsId)
            {
                if (player.GetComponent<Player>().isLocalPlayer) return;

                //player.UpdatePosition(serverPosition);
                //var playerPosition = player.transform.position;
                //if (Math.Abs(serverPosition.x - playerPosition.x) <= 0.1f ||
                //    Math.Abs(serverPosition.y - playerPosition.y) <= 0.1f ||
                //    Math.Abs(serverPosition.z - playerPosition.z) <= 0.1f)
                //    return;

                player.transform.position = serverPosition;
            }
            else
            {
                SpawnNewPlayer(playerId, $"new player {playerId}", serverPosition);
            }
        }

        public void SpawnNewPlayer(uint playerId, string playerName, Vector3 position, bool isLocalPlayer = false)
        {
            //var position = new Vector3(Random.Range(-mapDim, mapDim), playerPrefab.transform.localScale.y/2, 0);
            var player = Instantiate(playerPrefab, position, Quaternion.identity);
            //TODO: find out how correctly check if player is local
            player.GetComponent<Player>().SetInitialProperties(client, playerId, playerName, isLocalPlayer);

            playersDictionary.Add(playerId, player);
        }
    }


    [Serializable]
    public class MessageStructure
    {
        [SerializeField] public float X;
        [SerializeField] public float Y;
        [SerializeField] public float Z;

        [SerializeField] public int PlayerId;

        public MessageStructure(float x, float y, float z, int playerId)
        {
            X = x;
            Y = y;
            Z = z;
            PlayerId = playerId;
        }

        public static MessageStructure CreateFromJson(string jsonString)
        {
            return JsonUtility.FromJson<MessageStructure>(jsonString);
        }
    }
}
