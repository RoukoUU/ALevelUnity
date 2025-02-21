using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cainos.PixelArtTopDown_Basic
{
    public class TopDownCharacterController : MonoBehaviour
    {
        public float walkSpeed; // Normal walking speed
        public float sprintSpeed; // Speed when sprinting
        public float acceleration = 10f; // Smooth acceleration
        public float deceleration = 10f; // Smooth deceleration

        private Animator animator;
        private Rigidbody2D rb;
        private bool isSprinting = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Get movement input
            Vector2 dir = Vector2.zero;
            if (Input.GetKey(KeyCode.A))
            {
                dir.x = -1;
                animator.SetInteger("Direction", 3);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                dir.x = 1;
                animator.SetInteger("Direction", 2);
            }

            if (Input.GetKey(KeyCode.W))
            {
                dir.y = 1;
                animator.SetInteger("Direction", 1);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                dir.y = -1;
                animator.SetInteger("Direction", 0);
            }

            dir.Normalize();
            animator.SetBool("IsMoving", dir.magnitude > 0);

            // Check for sprint input
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isSprinting = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isSprinting = false;
            }

            // Determine target speed based on sprinting
            float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

            // Smoothly interpolate between current velocity and target velocity
            Vector2 targetVelocity = dir * targetSpeed;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.deltaTime);

            // Decelerate if no input
            if (dir.magnitude == 0)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.deltaTime);
            }
        }
    }
}