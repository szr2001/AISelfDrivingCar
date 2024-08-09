using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISelfDrivingCar.Handlers.Camera
{
    public class CameraControll : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float rotationSpeed = 150f;
        public float boostMultiplier = 2f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        private void HandleMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");   
            float moveY = 0f;

            if (Input.GetKey(KeyCode.Space))
            {
                moveY = 1f; 
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                moveY = -1f;
            }

            Vector3 move = transform.right * moveX + transform.up * moveY + transform.forward * moveZ;
            float currentSpeed = moveSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentSpeed *= boostMultiplier;
            }

            transform.position += move * currentSpeed * Time.deltaTime;
        }

        private void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            Vector3 rotation = transform.localEulerAngles;
            rotation.y += mouseX * rotationSpeed * Time.deltaTime; 
            rotation.x -= mouseY * rotationSpeed * Time.deltaTime; 

            rotation.x = rotation.x % 360;
            if (rotation.x > 180)
            {
                rotation.x -= 360;
            }
            else if (rotation.x < -180)
            {
                rotation.x += 360;
            }

            rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

            transform.localEulerAngles = rotation;
        }
    }
}
