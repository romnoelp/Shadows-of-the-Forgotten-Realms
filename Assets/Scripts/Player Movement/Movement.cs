using System.Collections;
using UnityEngine;

namespace romnoelp
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Movement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private BoxCollider2D playerBoxcastCollider;
        [SerializeField] private LayerMask ground;
        private float horizontalMovement;
        [SerializeField] private float movementSpeed = 4f;
        [SerializeField] private float jumpForce;
        private bool isJumping;
        [SerializeField] private float coyoteTime = .2f;
        private float coyoteTimeCounter;
        [SerializeField] private float jumpBufferTime = .2f;
        private float jumpBufferCounter; 
        private bool isFacingRight = true;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerBoxcastCollider = GetComponent<BoxCollider2D>();
        }

        private void Update() 
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
            
            // Coyote timer (Coyote -> The amount of time the player can press space after leaving the ground)
            if (IsGrounded()) 
            {
                coyoteTimeCounter = coyoteTime;
                Debug.Log("Grounded");
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if(Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x,  jumpForce);
                jumpBufferCounter = 0f;
            }

            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
                coyoteTimeCounter = 0f;
            }
        }


        // Physics for the movement
        private void FixedUpdate()
        {
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
                Vector3 localScale = transform.localScale;
                isFacingRight = !isFacingRight;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
    }

    // To do : Fix the rigid body and the controller because the player sticks to side surfaces when it jumps 
    
}

