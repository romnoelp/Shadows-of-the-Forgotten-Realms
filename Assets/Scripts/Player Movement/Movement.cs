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
        public bool isFacingRight = true;
        private bool canDash = true;
        private bool isDashing;
        private bool reachedJumpPeak = false;
        private DirectionBias directionBias;

        [Header ("Movement")]
        [SerializeField] private ParticleSystem movementTrailDust;
        [SerializeField] private float movementSpeed = 4f;
        
        [Header ("Jump")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float coyoteTime = .2f;
        [SerializeField] private float jumpBufferTime = .2f;
        [SerializeField] private float jumpCooldown = .4f;
        [SerializeField] private float fallSpeed = 2f;
        [SerializeField] private LayerMask ground;

        [Header ("Dashing")]
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float dashForce = 24f;
        [SerializeField] private float dashingTime = 0.2f;
        [SerializeField] private float dashingCooldown = 1f;

        [Header ("Camera Stuff")]
        [SerializeField] private GameObject cameraFollowObject;
        private float fallSpeedYDampingThreshold;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerBoxcastCollider = GetComponent<BoxCollider2D>();
            directionBias = cameraFollowObject.GetComponent<DirectionBias>();
            fallSpeedYDampingThreshold = Manager.instance.fallSpeedYDampingChangeThreshold;
        }

        private void Update() 
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            if (rb.velocity.y < fallSpeedYDampingThreshold && Manager.instance.isLerpingYDamping && 
            !Manager.instance.LerpedFromPlayerFalling)
            {
                Manager.instance.LerpYDamping(true);
            }
            if (rb.velocity.y >= 0f && !Manager.instance.isLerpingYDamping && Manager.instance.LerpedFromPlayerFalling)
            {
                Manager.instance.LerpedFromPlayerFalling = false;
            }
            
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
                rb.velocity = new Vector2(rb.velocity.x,  jumpForce * 1.5f);
                jumpBufferCounter = 0f;
                StartCoroutine(JumpCooldown());
            }

            if (rb.velocity.y <= 0f && !reachedJumpPeak && !isJumping)
            {
                reachedJumpPeak = true;
                StartCoroutine(IncreaseFallSpeed());
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
        }

        // Physics for the movement
        private void FixedUpdate()
        {
            FlipCheck();

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
            if (isFacingRight)
            {
                CreateTrailDust();
                Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                isFacingRight = !isFacingRight;
                directionBias.CallTurn();
            }
            else
            {
                CreateTrailDust();
                Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                isFacingRight = !isFacingRight;
                directionBias.CallTurn();
            }
        }

        private void FlipCheck()
        {
            if (horizontalMovement > 0f && !isFacingRight)
            {
                Flip();
            }
            else if (horizontalMovement < 0f && isFacingRight)
            {
                Flip();
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

            float dashDirection = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(dashDirection * dashForce, 0f);

            trailRenderer.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            trailRenderer.emitting = false;

            rb.gravityScale = originalGravity;
            isDashing = false;

            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

        private IEnumerator IncreaseFallSpeed()
        {
            while (rb.velocity.y < 0f)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallSpeed - 1) * Time.deltaTime;
                yield return null;
            }
            
            reachedJumpPeak = false;
        }
    }    
}

