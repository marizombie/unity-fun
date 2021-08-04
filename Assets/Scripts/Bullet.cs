using UnityEngine;

namespace Assets.Scripts
{
    class Bullet : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnCollisionEnter(Collision collision)
        {
            var hit = collision.gameObject;
            var hitPlayer = hit.GetComponent<Player>();
            if (hitPlayer != null)
            {
                Destroy(gameObject);
            }
            Debug.Log("Collision");

            //OnTriggerEnter
            //if (collider.CompareTag("bullet") && collider.GetComponent<Player>())
            //{
            //    DealDamage(gameObject);
            //    Destroy(collider.gameObject);
            //}
            //Debug.Log("Collision");
        }
    }
}
