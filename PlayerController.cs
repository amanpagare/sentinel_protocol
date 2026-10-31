using UnityEngine;
using UnityEngine.InputSystem;

namespace SentinelProtocol
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float rotationSpeed = 12f;
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private Transform cameraTransform;

        private CharacterController controller;
        private Vector3 verticalVelocity;

        public Transform CameraTransform
        {
            get => cameraTransform;
            set => cameraTransform = value;
        }

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || controller == null) return;

            Vector2 input = Vector2.zero;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1;
            input = Vector2.ClampMagnitude(input, 1f);

            Vector3 forward = cameraTransform ? cameraTransform.forward : Vector3.forward;
            Vector3 right = cameraTransform ? cameraTransform.right : Vector3.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 direction = forward * input.y + right * input.x;
            if (direction.sqrMagnitude > 1f) direction.Normalize();

            bool sprint = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            controller.Move(direction * (sprint ? sprintSpeed : moveSpeed) * Time.deltaTime);

            if (direction.sqrMagnitude > 0.001f)
            {
                var targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                    rotationSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && verticalVelocity.y < 0f) verticalVelocity.y = -2f;
            if (controller.isGrounded && keyboard.spaceKey.wasPressedThisFrame)
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity.y += gravity * Time.deltaTime;
            controller.Move(verticalVelocity * Time.deltaTime);
        }
    }
}
