using UnityEngine;

namespace Blooding.Runtime.Utility
{
    public sealed class DestroyWithDelay : MonoBehaviour
    {
        public float delay;
        private void Start() => Destroy(gameObject, delay);
    }
}
