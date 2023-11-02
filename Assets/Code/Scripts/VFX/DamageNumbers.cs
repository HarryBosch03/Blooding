using System;
using System.Collections.Generic;
using Blooding.Runtime.Core;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

namespace Blooding.Runtime.VFX
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class DamageNumbers : MonoBehaviour
    {
        public float lifetime = 2.0f;
        public Gradient color;
        public float startForce = 10.0f;
        public float gravity = -100.0f;

        private TextMeshProUGUI prefab;
        private Camera mainCam;
        private List<NumberInstance> instances = new();

        private void Awake()
        {
            mainCam = Camera.main;

            prefab = transform.Find("Prefab").GetComponent<TextMeshProUGUI>();
            prefab.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IDamageable.DamageEvent += OnDamage;
        }

        private void OnDisable()
        {
            IDamageable.DamageEvent -= OnDamage;
        }

        private void OnDamage(DamageArgs damage)
        {
            Spawn(damage.point, damage.damage);
        }

        private void LateUpdate()
        {
            var doomed = new List<NumberInstance>();
            foreach (var e in instances)
            {
                if (e.Update(mainCam, lifetime, color, gravity)) doomed.Add(e);
            }
            instances.RemoveAll(e => doomed.Contains(e));
        }

        public void Spawn(Vector3 point, int damage)
        {
            var sceneObject = Instantiate(prefab, transform);
            sceneObject.text = damage.ToString();
            instances.Add(new NumberInstance(sceneObject, point, damage, startForce));
        }

        public class NumberInstance
        {
            private TextMeshProUGUI sceneObject;
            private float startTime;
            private Vector3 point;
            private int damage;
            
            private Vector2 position;
            private Vector2 velocity;

            public NumberInstance(TextMeshProUGUI sceneObject, Vector3 point, int damage, float startForce)
            {
                this.point = point;
                this.sceneObject = sceneObject;
                this.damage = damage;
                startTime = Time.time;

                var a = (Random.value * 90.0f - 45.0f) * Mathf.Deg2Rad;
                velocity = new Vector2(Mathf.Sin(a), Mathf.Cos(a)) * startForce;
            }

            public bool Update(Camera camera, float lifetime, Gradient color, float gravity)
            {
                var t = (Time.time - startTime) / lifetime;
                if (t > 1.0f)
                {
                    Destroy(sceneObject.gameObject);
                    return true;
                }

                var screenPoint = camera.WorldToScreenPoint(point) + (Vector3)position;
                sceneObject.rectTransform.position = screenPoint;
                sceneObject.color = color.Evaluate(t);
                
                position += velocity * Time.deltaTime;
                velocity += Vector2.up * gravity * Time.deltaTime;
                
                return false;
            }
        }
    }
}
