using System.Collections;
using UnityEngine;

namespace romnoelp
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Movement : MonoBehaviour
    {
        private Rigidbody2D rigidbody2D;
        public Rigidbody2D rb 
        {
            get{return rb;}
            private set {rb = value;}
        }
        private BoxCollider2D playerBoxcastCollider;
        private float horizontalMovement;
         
        public bool isFacingRight = true;
        private bool canDash = true;
        private bool isDashing;
        private bool reachedJumpPeak = false;
        private DirectionBias directionBias;

        [Header ("Movement")]
        [SerializeField] private float movementSpeed = 4f;
        
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
            rigidbody2D = GetComponent<Rigidbody2D>();
            playerBoxcastCollider = GetComponent<BoxCollider2D>();
            directionBias = cameraFollowObject.GetComponent<DirectionBias>();
            fallSpeedYDampingThreshold = Manager.instance.fallSpeedYDampingChangeThreshold;
        }

        private void Update() 
        {
            horizontalMovement = Input.GetAxisRaw("Horizontal");

            if (rigidbody2D.velocity.y < fallSpeedYDampingThreshold && !Manager.instance.isLerpingYDamping && 
            !Manager.instance.LerpedFromPlayerFalling)
            {
                Manager.instance.LerpYDamping(true);
            }
            if (rigidbody2D.velocity.y >= 0f && !Manager.instance.isLerpingYDamping && Manager.instance.LerpedFromPlayerFalling)
            {
                Manager.instance.LerpedFromPlayerFalling = false;
                Manager.instance.LerpYDamping(false);
            }
            
            if (isDashing)
            {
                return; // -> If the player is indeed dashing, this condition will just return and make it s othat the player 
                        //    can't make any movements aside from the dash
            }

            // Jumping stuff
            // Coyote timer (Coyote -> The amount of time the player can press space after leaving the ground)
            

            // Dashing stuff 
            if (Input.GetKey(KeyCode.LeftShift) && canDash)
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

            rigidbody2D.velocity = new Vector2(horizontalMovement * movementSpeed, rigidbody2D.velocity.y);
        }

        // Creates a box cast, a collider below the player, and will return true if it detects a collision between the specified layer 
        

        // Flips the character when the horizontal movement becomes negative (Note: On a plane, negative <---- 0 ----> positive)
        private void Flip()
        {
            if (isFacingRight)
            {
                Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
                transform.rotation = Quaternion.Euler(rotator);
                isFacingRight = !isFacingRight;
                directionBias.CallTurn();
            }
            else
            {
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
        
  

        private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rigidbody2D.gravityScale;
            rigidbody2D.gravityScale = 0f;

            float dashDirection = isFacingRight ? 1f : -1f; // Use isFacingRight to determine the direction
            rigidbody2D.velocity = new Vector2(dashDirection * dashForce, 0f);

            trailRenderer.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            trailRenderer.emitting = false;

            rigidbody2D.gravityScale = originalGravity;
            isDashing = false;

            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

        
    }    
}

