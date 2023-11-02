using UnityEngine;
using UnityEngine.InputSystem;

namespace Blooding.Runtime.Player
{
    [SelectionBase, DisallowMultipleComponent]
    public sealed class PlayerMovement : MonoBehaviour
    {
        public InputActionAsset inputAsset;

        public float playerHeight = 1.8f;
        public float stepHeight = 0.2f;
        public float moveSpeed = 12.0f;
        public float accelerationTime = 0.08f;
        [Range(0.0f, 1.0f)] public float airAccelerationPenalty = 0.8f;
        public float jumpHeight = 2.5f;
        public float mouseSensitivity = 0.3f;
        public float gravityScale = 3.0f;
        public float viewSmoothing = 0.8f;

        private InputActionReference moveAction;
        private InputActionReference jumpAction;

        private Rigidbody body;
        private bool isOnGround;
        private RaycastHit groundHit;
        
        private Camera mainCam;
        private Transform view;
        private Quaternion viewRotation;
        private Vector2 cameraRotation;
        private bool jumpFlag;

        public Vector3 Gravity => Physics.gravity * gravityScale;
        
        private void Awake()
        {
            body = gameObject.AddComponent<Rigidbody>();
            body.drag = 0.0f;
            body.angularDrag = 0.0f;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            body.interpolation = RigidbodyInterpolation.None;
            body.constraints = RigidbodyConstraints.FreezeRotation;

            var collider = gameObject.AddComponent<CapsuleCollider>();
            collider.radius = 0.2f;
            collider.height = playerHeight - stepHeight;
            collider.center = Vector3.up * (playerHeight + stepHeight) / 2.0f;

            mainCam = Camera.main;

            view = transform.Find("View");
            
            inputAsset.Enable();
            
            moveAction = bind("Move");
            jumpAction = bind("Jump");
            
            InputActionReference bind(string path) => InputActionReference.Create(inputAsset.FindAction(path));
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (jumpAction.action.WasPressedThisFrame()) jumpFlag = true;
            
            var cameraRotationDelta = Vector2.zero;

            cameraRotationDelta += (Mouse.current?.delta.ReadValue() ?? Vector2.zero) * mouseSensitivity;
            
            cameraRotation += cameraRotationDelta;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -90.0f, 90.0f);
            
            mainCam.transform.position = transform.position + Vector3.up * playerHeight;
            mainCam.transform.rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0.0f);

            viewRotation = Quaternion.Slerp(viewRotation, mainCam.transform.rotation, viewSmoothing * Time.deltaTime);
            
            view.transform.position = mainCam.transform.position;
            view.transform.rotation = viewRotation;
        }

        private void FixedUpdate()
        {
            CheckForGround();
            Move();
            Jump();

            jumpFlag = false;
            
            transform.rotation = Quaternion.Euler(0.0f, cameraRotation.x, 0.0f);
            if (body.useGravity) body.AddForce(Gravity - Physics.gravity, ForceMode.Acceleration);
        }

        private void CheckForGround()
        {
            var ray = new Ray(body.position + Vector3.up, Vector3.down);
            isOnGround = Physics.Raycast(ray, out groundHit, 1.0f);
            if (!isOnGround) return;
            
            body.position += groundHit.normal * Vector3.Dot(groundHit.normal, groundHit.point - body.position);
            body.velocity += groundHit.normal * Mathf.Max(0.0f, -Vector3.Dot(groundHit.normal, body.velocity));
        }

        private void Move()
        {
            var acceleration = 2.0f / accelerationTime;
            if (!isOnGround) acceleration *= 1.0f - airAccelerationPenalty;
            
            var input = moveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            var target = transform.TransformVector(input.x, 0.0f, input.y) * moveSpeed;
            var force = (target - body.velocity) * acceleration;
            force.y = 0.0f;
            
            body.AddForce(force);
        }

        private void Jump()
        {
            if (!jumpFlag) return;
            if (!isOnGround) return;

            var gravity = -Gravity.y;
            var force = Vector3.up * Mathf.Sqrt(2.0f * gravity * jumpHeight);
            body.AddForce(force, ForceMode.VelocityChange);
        }
    }
}
