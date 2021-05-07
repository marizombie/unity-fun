using System.Collections;
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
        public bool isLocalPlayer;
        private string playerName;
        private Vector3 currentServerPosition;
        private Client client;

        public bool IsUpdatePositionNeeded;
        public GameObject BulletPrefab;

        public void SetInitialProperties(Client client, int playerId, string playerName, bool isLocal)
        {
            Debug.Log(client);
            if (client == null) return;

            this.client = client;
            this.playerId = playerId;
            this.playerName = playerName;
            isLocalPlayer = isLocal;
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            if (isLocalPlayer) return;
            IsUpdatePositionNeeded = true;
            currentServerPosition = newPosition;
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
            if (!isLocalPlayer) return;
            var message = new MessageStructure(position.x, position.y, position.z, playerId);
            var serializedMessage = JsonUtility.ToJson(message);
            var bytes = Encoding.Unicode.GetBytes(serializedMessage);
            Debug.Log(client);
            client.SendMessage(bytes);
        }

        IEnumerator MoveFunction()
        {
            var timeSinceStarted = 0f;
            while (true)
            {
                timeSinceStarted += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, currentServerPosition, timeSinceStarted);

                // If the object has arrived, stop the coroutine
                if (transform.position == currentServerPosition)
                {
                    IsUpdatePositionNeeded = false;
                    yield break;
                }

                // Otherwise, continue next frame
                yield return null;
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
        }

        private void FixedUpdate()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (!isLocalPlayer) return;

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

            if (IsUpdatePositionNeeded)
            {
                StartCoroutine("MoveFunction");
            }
        }
    }
}