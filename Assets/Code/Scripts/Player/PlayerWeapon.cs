using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Blooding.Runtime.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerWeapon : MonoBehaviour
    {
        public InputActionAsset inputAsset;
        public GameObject knifeProjectile;
        public GameObject knifeModel;
        public Vector3 knifePosition0;
        public Vector3 knifePosition1;
        public Vector3 lookAtPosition;
        public float knifeRechargeTime;

        public int maxKnives = 4;
        public int knifeCount;
        public float bobFrequency;
        public float bobAmplitude;
        public float knifeAnimationDuration = 0.2f;
        public AnimationCurve knifeAnimationCurve;

        private InputActionReference useAction;
        private bool useFlag;
        private float knifeRechargeTimer;
        private float[] knifeAnimationTimers;

        private Camera mainCamera;
        private Quaternion rotation;

        private GameObject[] knifes;

        private void Awake()
        {
            mainCamera = Camera.main;

            inputAsset.Enable();
            useAction = InputActionReference.Create(inputAsset.FindAction("Use"));

            knifes = new GameObject[4];
            knifes[0] = knifeModel;

            var parent = new GameObject("Knives").transform;
            parent.SetParent(knifeModel.transform.parent);
            parent.localPosition = Vector3.zero;
            parent.localRotation = Quaternion.identity;

            for (var i = 0; i < knifes.Length; i++)
            {
                if (i > 0) knifes[i] = Instantiate(knifeModel, knifeModel.transform.parent);
                knifes[i].transform.SetParent(parent);
                foreach (var t in knifes[i].GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = 3;
                }
            }

            knifeAnimationTimers = new float[4];
            knifeCount = maxKnives;
        }

        private void Update()
        {
            if (useAction.action.WasPerformedThisFrame()) useFlag = true;
        }

        private void FixedUpdate()
        {
            if (knifeRechargeTimer < 0.0f)
            {
                knifeCount = maxKnives;
            }

            knifeRechargeTimer -= Time.deltaTime;

            if (useFlag && knifeCount > 0)
            {
                knifeRechargeTimer = knifeRechargeTime;
                knifeCount--;
                knifeAnimationTimers[knifeCount % 4] = 0.0f;
                var sp = knifes[knifeCount % 4].transform;
                Instantiate(knifeProjectile, mainCamera.transform.position + mainCamera.transform.forward * 0.5f, mainCamera.transform.rotation);
            }
            
            for (var i = 0; i < knifes.Length; i++)
            {
                if (i < knifeCount)
                {
                    knifeAnimationTimers[i] += Time.deltaTime;
                }
                else
                {
                    knifeAnimationTimers[i] = 0.0f;
                }
            }

            useFlag = false;
        }

        private void LateUpdate()
        {
            var bob = Mathf.Sin(Time.time * Mathf.PI * bobFrequency) * bobAmplitude;

            var knifeOffsets = new []
            {
                knifePosition0,
                new (-knifePosition0.x, knifePosition0.y, knifePosition0.z),
                knifePosition1,
                new (-knifePosition1.x, knifePosition1.y, knifePosition1.z),
            };

            for (var i = 0; i < knifes.Length; i++)
            {
                var t = Mathf.Clamp01(knifeAnimationTimers[i] / knifeAnimationDuration - i * 0.2f);
                t = knifeAnimationCurve.Evaluate(t);
                knifes[i].transform.localScale = Vector3.one * t;

                var localPos = knifeOffsets[i] + Vector3.up * bob;
                knifes[i].transform.localPosition = localPos;

                var lookAt = mainCamera.transform.TransformPoint(lookAtPosition);
                var rotation = Quaternion.LookRotation(lookAt - knifes[i].transform.position, mainCamera.transform.up);
                knifes[i].transform.rotation = rotation;
            }
        }
    }
}