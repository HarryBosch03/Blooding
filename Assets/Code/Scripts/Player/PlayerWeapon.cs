using System;
using Blooding.Runtime.Utility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Blooding.Runtime.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerWeapon : MonoBehaviour
    {
        public InputActionAsset inputAsset;
        public GameObject projectile;
        [Range(0.0f, 1.0f)] public float energy;
        public float energyRechargeTime;
        public float maxEnergy;
        public float castEnergy;
        public float castDelay;
        public Vector3 spawnOffset;

        [Space]
        [Header("Animation")]
        [Range(0.0f, 1.0f)] 
        public float smoothing;
        
        private InputActionReference useAction;
        private Animator model;
        private Quaternion modelRotation;

        private Camera mainCamera;
        private PlayerMovement movement;
        private Quaternion lastCameraRotation;
        private float lastCastTime;

        private void Awake()
        {
            mainCamera = Camera.main;
            movement = GetComponent<PlayerMovement>();

            inputAsset.Enable();
            useAction = InputActionReference.Create(inputAsset.FindAction("Use"));

            model = transform.Find<Animator>("Hands/Hands");
        }

        private void LateUpdate()
        {
            model.transform.position = mainCamera.transform.position;
            
            var cameraRotation = mainCamera.transform.rotation;
            modelRotation = Quaternion.Slerp(modelRotation, cameraRotation, Time.deltaTime / Mathf.Max(Time.deltaTime, smoothing));
            model.transform.rotation = modelRotation;

            model.SetFloat("movement", movement.CurrentSpeed);
            model.SetBool("grounded", movement.IsOnGround);
        }

        private void FixedUpdate()
        {
            if (useAction.action.IsPressed() && Time.time > lastCastTime + castDelay && energy * maxEnergy >= castEnergy)
            {
                Instantiate
                (
                    projectile,
                    mainCamera.transform.TransformPoint(spawnOffset),
                    mainCamera.transform.rotation
                );
                
                model.SetTrigger("shoot");
                
                energy -= castEnergy / maxEnergy;
                lastCastTime = Time.time;
            }

            if (energy < 1.0f) energy += Time.deltaTime / energyRechargeTime / maxEnergy;
            else energy = 1.0f;
        }
    }
}