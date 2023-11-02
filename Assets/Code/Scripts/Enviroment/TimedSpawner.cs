using UnityEngine;

namespace Blooding.Runtime.Enviroment
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class TimedSpawner : MonoBehaviour
    {
        public GameObject prefab;
        public float delay;

        private float spawnTimer;

        private void Update()
        {
            if (Time.time > spawnTimer)
            {
                spawnTimer += delay;
                Instantiate(prefab, transform.position, transform.rotation);
            }
            spawnTimer -= Time.deltaTime;
        }
    }
}
