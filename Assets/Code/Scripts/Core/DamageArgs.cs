using UnityEngine;

namespace Blooding.Runtime.Core
{
    [System.Serializable]
    public struct DamageArgs
    {
        public int damage;
        public Vector3 point;

        public DamageArgs(int damage, Vector3 point)
        {
            this.damage = damage;
            this.point = point;
        }
    }
}