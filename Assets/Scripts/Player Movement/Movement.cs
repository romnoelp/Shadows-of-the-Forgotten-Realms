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
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            playerBoxcastCollider = GetComponent<BoxCollider2D>();
        }

        private void Update() 
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector2(horizontalMovement * movementSpeed, rb.velocity.y);
        }

        private bool IsGrounded() 
        {
            return Physics2D.BoxCast(playerBoxcastCollider.bounds.center, playerBoxcastCollider.bounds.size, 0f, Vector2.down, 1f, ground);
        }
    }

    // To do : Fix the rigid body and the controller because the player sticks to side surfaces when it jumps 
    
}

