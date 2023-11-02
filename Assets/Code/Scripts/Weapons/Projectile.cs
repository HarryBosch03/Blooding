using Blooding.Runtime.Core;
using UnityEngine;

namespace Blooding.Runtime.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public int damage;
        public float speed;
        public float maxDistance;
        public ParticleSystem.MinMaxCurve gravityScale = 1.0f;
        public float detachLifetime;
        public GameObject impactPrefab;

        private Transform detach;
        private float distanceTraveled;

        private Vector3 velocity;

        private void Awake()
        {
            velocity = transform.forward * speed;
            detach = transform.Find("Detach");
        }

        private void FixedUpdate()
        {
            var speed = velocity.magnitude;
            var step = speed * Time.deltaTime;

            var ray = new Ray(transform.position, velocity);
            if (Physics.Raycast(ray, out var hit, step * 1.05f))
            {
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageable.Damage(new DamageArgs(damage, hit.point));
                }
                
                if (detach)
                {
                    detach.SetParent(null);
                    Destroy(detach, detachLifetime);
                }

                if (impactPrefab) Instantiate(impactPrefab, hit.point, transform.rotation);
                Destroy(gameObject);
            }

            distanceTraveled += step;
            if (distanceTraveled > maxDistance)
            {
                Destroy(gameObject);
            }

            transform.position += velocity * Time.deltaTime;
            velocity += Physics.gravity * gravityScale.Evaluate(distanceTraveled / maxDistance) * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(velocity, transform.up);
        }
    }
}