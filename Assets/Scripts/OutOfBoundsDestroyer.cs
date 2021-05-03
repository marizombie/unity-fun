using UnityEngine;

namespace Assets.Scripts
{
    public class OutOfBoundsDestroyer : MonoBehaviour
    {
        private readonly float topBound = 100;
        private readonly float lowerBound = -20;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (transform.position.z > topBound)
            {
                Destroy(gameObject);
            }
            else if (transform.position.z < lowerBound)
            {
                Debug.Log("Game Over!");
                Destroy(gameObject);
            }
        }
    }
}
