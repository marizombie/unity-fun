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
        public List<int> playerIds;

        private void Start()
        {
            playerIds = new List<int>();

            client = new Client();
            client.ReceiveMessage();

            SpawnNewPlayer(0, "initial player");
        }

        private void Update()
        {
            if (client.MessageDataStorage.IsEmpty) return;
            client.MessageDataStorage.TryDequeue(out var results);
            var jsonStr = Encoding.Unicode.GetString(results);
            var receivedMessage = JsonUtility.FromJson<MessageStructure>(jsonStr);
            var playerId = receivedMessage.PlayerId;
            if (playerIds.Contains(playerId)) return;

            SpawnNewPlayer(playerId, $"new player {playerId}");
            //Debug.Log($"{receivedMessage.X}, {receivedMessage.Y}");
        }

        public void SpawnNewPlayer(int playerId, string playerName = "")
        {
            playerIds.Add(playerId);

            var position = new Vector3(Random.Range(-mapDim, mapDim), playerPrefab.transform.localScale.y/2, 0);
            var player = Instantiate(playerPrefab, position, Quaternion.identity);
            player.GetComponent<Player>().SetInitialProperties(client, playerId, playerName);
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
