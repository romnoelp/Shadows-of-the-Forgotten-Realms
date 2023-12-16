using System.Collections;
using UnityEngine;

namespace romnoelp
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Movement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private BoxCollider2D playerBoxcastCollider;
        private float horizontalMovement;
        private bool isJumping;
        private float coyoteTimeCounter;
        private float jumpBufferCounter; 
        private bool isFacingRight = true;
        private bool canDash = true;
        private bool isDashing;

        [Header ("Movement")]
        [SerializeField] private ParticleSystem movementTrailDust;
        [SerializeField] private float movementSpeed = 4f;
        
        [Header ("Jump")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float coyoteTime = .2f;
        [SerializeField] private float jumpBufferTime = .2f;
        [SerializeField] private float jumpCooldown = .4f;
        [SerializeField] private LayerMask ground;

        [Header ("Dashing")]
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float dashForce = 24f;
        [SerializeField] private float dashingTime = 0.2f;
        [SerializeField] private float dashingCooldown = 1f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerBoxcastCollider = GetComponent<BoxCollider2D>();
        }

        private void Update() 
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            
            if (isDashing)
            {
                return; // -> If the player is indeed dashing, this condition will just return and make it s othat the player 
                        //    can't make any movements aside from the dash
            }

            // Jumping stuff
            // Coyote timer (Coyote -> The amount of time the player can press space after leaving the ground)
            if (IsGrounded()) 
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if(Input.GetButtonDown("Jump"))
            {
                CreateTrailDust();
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x,  jumpForce);
                jumpBufferCounter = 0f;
                StartCoroutine(JumpCooldown());
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
                coyoteTimeCounter = 0f;
            }

            // Dashing stuff 
            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(Dash());
            }

            Flip();
        }

        // Physics for the movement
        private void FixedUpdate()
        {
            if (isDashing)
            {
                return;
            }

            rb.velocity = new Vector2(horizontalMovement * movementSpeed, rb.velocity.y);
        }

        // Creates a box cast, a collider below the player, and will return true if it detects a collision between the specified layer 
        private bool IsGrounded() 
        {
            return Physics2D.BoxCast(playerBoxcastCollider.bounds.center, playerBoxcastCollider.bounds.size, 0f, Vector2.down, .1f, ground);
        }

        // Flips the character when the horizontal movement becomes negative (Note: On a plane, negative <---- 0 ----> positive)
        private void Flip()
        {
            if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
            {
                CreateTrailDust();
                Vector3 localScale = transform.localScale;
                isFacingRight = !isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

        private void CreateTrailDust() 
        {
            movementTrailDust.Play();
        }

        private IEnumerator JumpCooldown()
        {
            isJumping = true;
            yield return new WaitForSeconds(jumpCooldown);
            isJumping = false;
        }

        private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * dashForce, 0f);
            trailRenderer.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            trailRenderer.emitting = false;
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
    }    
}

