using System.Collections;
using UnityEngine;

namespace romnoelp
{
    public class Jump : MonoBehaviour
    {
        private Movement movementScript;
        private bool isJumping;
        private bool reachedJumpPeak = false;

        private float coyoteTimeCounter;
        private float jumpBufferCounter;
        private BoxCollider2D playerBoxcastCollider;
        private Rigidbody2D rb;
        [Header ("Jump")]
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float coyoteTime = .2f;
        [SerializeField] private float jumpBufferTime = .2f;
        [SerializeField] private float jumpCooldown = .4f;
        [SerializeField] private float fallSpeed = 2f;
        [SerializeField] private LayerMask ground;
        [SerializeField] private ParticleSystem movementTrailDust;


        private void Start() {
            movementScript = GetComponent<Movement>();
            rb = movementScript.rb;
            playerBoxcastCollider = GetComponent<BoxCollider2D>();
        }
        private void Update() {
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
                // CreateTrailDust();
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
        }

        private bool IsGrounded() 
        {
            return Physics2D.BoxCast(playerBoxcastCollider.bounds.center, playerBoxcastCollider.bounds.size, 0f, Vector2.down, .1f, ground);
        }
        
        private IEnumerator JumpCooldown()
        {
            isJumping = true;
            yield return new WaitForSeconds(jumpCooldown);
            isJumping = false;
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

