using UnityEngine;

namespace Blooding.Runtime.Core
{
    public class Dummy : MonoBehaviour, IDamageable
    {
        public void Damage(DamageArgs args)
        {
            IDamageable.OnDamage(args);
        }
    }
}