using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public Camera mainCamera;

    bool facingRight = true;
    float moveDirection = 0;
    bool isGrounded = false;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;

    float leftPressStartTime = 0f;
    bool leftPressed = false;
    float rightPressStartTime = 0f;
    bool rightPressed = false;

    // DASH STUFF 
    public bool canDash = true; // If player is allowed to dash
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f; // Cooldown
    private bool isDashing = false; // Whether the player is currently dashing
    private float dashEndTime = 0f;
    private float nextDashTime = 0f;

    // Invulnerability
    private bool isInvulnerable = false; // Whether the player is invulnerable during dash

    // Trail Renderer
    public TrailRenderer dashTrail; // Reference to the TrailRenderer for dash effect

    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }

        // Ensure the TrailRenderer is disabled at the start
        if (dashTrail != null)
        {
            dashTrail.enabled = false;
        }
    }

    void Update()
    {
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)))
        {
            moveDirection = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? -1 : 1;
        }
        else
        {
            moveDirection = 0;
        }

        if (moveDirection != 0)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            r2d.linearVelocity = new Vector2(r2d.linearVelocity.x, jumpHeight);
        }

        if (mainCamera)
        {
            mainCamera.transform.position = new Vector3(t.position.x, cameraPos.y, cameraPos.z);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftPressed = true;
            leftPressStartTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            leftPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightPressed = true;
            rightPressStartTime = Time.time;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rightPressed = false;
        }

        if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time >= nextDashTime)
        {
            StartDash();
        }

        if (isDashing && Time.time >= dashEndTime)
        {
            EndDash();
        }
    }

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min
                                 + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (!isDashing)
        {
            r2d.linearVelocity = new Vector2(moveDirection * maxSpeed, r2d.linearVelocity.y);
        }

        float pressDuration = 0f;
        if (leftPressed)
        {
            pressDuration = Time.time - leftPressStartTime;
        }
        else if (rightPressed)
        {
            pressDuration = Time.time - rightPressStartTime;
        }

        // Decide how to scale velocity:
        // e.g., short press < 0.2 => half speed, else normal to 1.5x speed
        // You can tweak these thresholds and multipliers to taste
        if (pressDuration > 0f && !isDashing)
        {
            float multiplier;
            if (pressDuration < 0.2f)
            {
                multiplier = 0.5f;
            }
            else if (pressDuration < 1.0f)
            {
                multiplier = 1.0f;
            }
            else
            {
                multiplier = 1.5f;
            }

            var v = r2d.linearVelocity;
            v.x *= multiplier;
            r2d.linearVelocity = v;
        }

        // Simple debug lines
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }

    void StartDash()
    {
        isDashing = true;
        isInvulnerable = true; // Player becomes invulnerable
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;

        float dashDirection = moveDirection != 0 ? moveDirection : (facingRight ? 1 : -1);
        r2d.linearVelocity = new Vector2(dashDirection * dashSpeed, r2d.linearVelocity.y);

        // Enable TrailRenderer for dash effect
        if (dashTrail != null)
        {
            dashTrail.enabled = true;
        }
    }

    void EndDash()
    {
        isDashing = false;
        isInvulnerable = false; 

        r2d.linearVelocity = new Vector2(moveDirection * maxSpeed, r2d.linearVelocity.y);

        // Trailrenderer > particle system 
        if (dashTrail != null)
        {
            dashTrail.enabled = false;
        }
    }

    public void TakeDamage()
    {
        if (!isInvulnerable)
        {
            Debug.Log("Player took damage!");
        }
        else
        {
            Debug.Log("Player is invulnerable during dash!");
        }
    }
}