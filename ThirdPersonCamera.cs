using UnityEngine;
using UnityEngine.InputSystem;

namespace SentinelProtocol
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 5f;
        [SerializeField] private float mouseSensitivity = 0.12f;
        [SerializeField] private float minPitch = -25f;
        [SerializeField] private float maxPitch = 65f;
        [SerializeField] private float followSmoothTime = 0.06f;

        private float yaw;
        private float pitch = 15f;
        private Vector3 followVelocity;

        public Transform Target { get => target; set => target = value; }

        private void Start()
        {
            if (!target)
            {
                Debug.LogError("ThirdPersonCamera needs a target.", this);
                enabled = false;
                return;
            }
            yaw = target.eulerAngles.y;
            LockCursor(true);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard == null || mouse == null) return;

            if (keyboard.escapeKey.wasPressedThisFrame) LockCursor(false);
            if (mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
                LockCursor(true);

            if (Cursor.lockState != CursorLockMode.Locked) return;

            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * mouseSensitivity;
            pitch = Mathf.Clamp(pitch - delta.y * mouseSensitivity, minPitch, maxPitch);
        }

        private void LateUpdate()
        {
            if (!target) return;
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desired = target.position - rotation * Vector3.forward * distance;
            transform.position = Vector3.SmoothDamp(transform.position, desired,
                ref followVelocity, followSmoothTime);
            transform.rotation = rotation;
        }

        private void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
