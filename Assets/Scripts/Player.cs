using System;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        private float speed = 15;
        private float turnSpeed = 30;
        private float horizontalInput;
        private float forwardInput;
        private Client client;

        public int PlayerId;
        public GameObject bulletPrefab;

        private void SetPlayerId(int id)
        {
            PlayerId = id;
        }

        private void Start()
        {
            client = new Client();
            client.ReceiveMessage();
        }

        // Update is called once per frame
        private void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");

            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput); 
            // transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

            if (Input.GetMouseButtonDown(0))
            {
                Instantiate(bulletPrefab, transform.position, bulletPrefab.transform.rotation);
            }

            var pos = transform.position;
            var message = new MessageStructure(pos.x, pos.y, pos.z, PlayerId);
            var serializedMessage = JsonUtility.ToJson(message);
            var bytes = Encoding.Unicode.GetBytes(serializedMessage);
            client.SendMessage(bytes);

            client.messageDataStorage.TryDequeue(out var results);
            var jsonStr = Encoding.Unicode.GetString(results);
            //Debug.Log(jsonStr);
            var receivedMessage = JsonUtility.FromJson<MessageStructure>(jsonStr);
            Debug.Log($"{receivedMessage.X}, {receivedMessage.Y}");
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