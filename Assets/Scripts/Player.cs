using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private const float Speed = 15;
        private const float BulletSpeed = 500;

        //private float turnSpeed = 30;
        private float horizontalInput;
        private float forwardInput;

        private int playerId;
        private string playerName;
        private Client client;

        public GameObject BulletPrefab;

        public void SetInitialProperties(Client client, int playerId, string playerName)
        {
            this.client = client;
            this.playerId = playerId;
            this.playerName = playerName;
        }

        private void Shoot(Vector3 position)
        {
            var targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var bullet = Instantiate(BulletPrefab, position, BulletPrefab.transform.rotation);
            bullet.transform.LookAt(targetPoint);
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * BulletSpeed);
        }

        private void SyncWithServer(Vector3 position)
        {
            var message = new MessageStructure(position.x, position.y, position.z, playerId);
            var serializedMessage = JsonUtility.ToJson(message);
            var bytes = Encoding.Unicode.GetBytes(serializedMessage);
            client.SendMessage(bytes);
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");

            transform.Translate(Vector3.forward * Time.deltaTime * Speed * forwardInput);
            transform.Translate(Vector3.right * Time.deltaTime * Speed * horizontalInput);
            //transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

            var position = transform.position;

            if (Input.GetMouseButtonDown(0))
            {
                Shoot(position);
            }

            SyncWithServer(position);

            //client.messageDataStorage.TryDequeue(out var results);
            //var jsonStr = Encoding.Unicode.GetString(results);
            //var receivedMessage = JsonUtility.FromJson<MessageStructure>(jsonStr);
            //Debug.Log($"{receivedMessage.X}, {receivedMessage.Y}");
        }
    }
}