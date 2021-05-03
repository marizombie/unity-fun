using UnityEngine;

namespace Assets.Scripts
{
    class CollisionsDetector : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Collision");
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
