﻿using System.Collections;
using UnityEngine;
using FlatBuffers;
using Messages;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private const float Speed = 15;
        private const float BulletSpeed = 500;

        //private float turnSpeed = 30;
        private float horizontalInput;
        private float forwardInput;

        private uint playerId;
        public bool isLocalPlayer;
        private string playerName;
        private Vector3 previousPlayerPosition;
        private Vector3 currentServerPosition;
        private Client client;

        public bool IsUpdatePositionNeeded;
        public GameObject BulletPrefab;

        public void SetInitialProperties(Client client, uint playerId, string playerName, bool isLocal)
        {
            if (client == null) Debug.Log("client is null");

            this.client = client;
            this.playerId = playerId;
            this.playerName = playerName;
            isLocalPlayer = isLocal;
        }

        //public void UpdatePosition(Vector3 newPosition)
        //{
        //    if (isLocalPlayer) return;

        //    Debug.Log($"opponent position updating, new position: {newPosition}");
        //    IsUpdatePositionNeeded = true;
        //    currentServerPosition = newPosition;
        //}

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

            var flatBufBuilder = new FlatBufferBuilder(1);
            Message.CreateMessage(flatBufBuilder, playerId, position.z, position.x, position.y);
            using var memoryStream = flatBufBuilder.DataBuffer.ToMemoryStream(flatBufBuilder.DataBuffer.Position, flatBufBuilder.DataBuffer.Length);
            client.SendMessage(memoryStream.ToArray());
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

            if (previousPlayerPosition != position)
            {
                SyncWithServer(position);
            }

            previousPlayerPosition = position;

            //if (IsUpdatePositionNeeded)
            //{
            //    StartCoroutine("MoveFunction");
            //}
        }

        //IEnumerator MoveFunction()
        //{
        //    var timeSinceStarted = 0f;
        //    while (true)
        //    {
        //        timeSinceStarted += Time.deltaTime;
        //        transform.position = Vector3.Lerp(transform.position, currentServerPosition, timeSinceStarted);

        //        // If the object has arrived, stop the coroutine
        //        if (transform.position == currentServerPosition)
        //        {
        //            IsUpdatePositionNeeded = false;
        //            yield break;
        //        }

        //        // Otherwise, continue next frame
        //        yield return null;
        //    }
        //}
    }
}