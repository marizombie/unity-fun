using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class PlayerSpawnManager : MonoBehaviour
    {
        private float mapDim = 20;
        private Client client;

        public GameObject playerPrefab;
        public Dictionary<int, Player> playersDictionary;

        private void Start()
        {
            playersDictionary = new Dictionary<int, Player>();

            client = new Client();
            client.ReceiveMessage();

            var startingPosition = new Vector3(Random.Range(-mapDim, mapDim), playerPrefab.transform.localScale.y / 2, 0);
            SpawnNewPlayer(0, "initial player", startingPosition);
        }

        private void Update()
        {
            if (client.MessageDataStorage.IsEmpty) return;
            client.MessageDataStorage.TryDequeue(out var results);
            var jsonStr = Encoding.Unicode.GetString(results);
            var receivedMessage = JsonUtility.FromJson<MessageStructure>(jsonStr);
            var playerId = receivedMessage.PlayerId;
            var serverPosition = new Vector3(receivedMessage.X, receivedMessage.Y, receivedMessage.Z);

            var containsId = playersDictionary.TryGetValue(playerId, out var player);
            if (containsId)
            {
                player.UpdatePosition(serverPosition);
            }
            else
            {
                SpawnNewPlayer(playerId, $"new player {playerId}", serverPosition);
            }
        }

        public void SpawnNewPlayer(int playerId, string playerName, Vector3 position)
        {
            //var position = new Vector3(Random.Range(-mapDim, mapDim), playerPrefab.transform.localScale.y/2, 0);
            var player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.GetComponent<Player>().SetInitialProperties(client, playerId, playerName);

            playersDictionary.Add(playerId, player.GetComponent<Player>());
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
