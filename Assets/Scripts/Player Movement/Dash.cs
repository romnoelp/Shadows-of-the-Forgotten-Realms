using System.Collections;
using UnityEngine;

namespace romnoelp
{
    public class Dash : MonoBehaviour
    {
        
        
        [Header ("Dashing")]
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float dashForce = 24f;
        [SerializeField] private float dashingTime = 0.2f;
        [SerializeField] private float dashingCooldown = 1f;
        public bool isFacingRight = true;
        private bool canDash = true;
        private bool isDashing;
        private Movement movementScript;
        private Rigidbody2D rb;


        private void Start() {
            movementScript = GetComponent<Movement>();
            rb = movementScript.rb;
        }

        private void Update() {
            if (Input.GetKey(KeyCode.LeftShift) && canDash)
            {
                StartCoroutine(DashMovement());
            }
        }

        private IEnumerator DashMovement()
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;

            float dashDirection = isFacingRight ? 1f : -1f; // Use isFacingRight to determine the direction
            rb.velocity = new Vector2(dashDirection * dashForce, 0f);

            yield return new WaitForSeconds(dashingTime);

            rb.gravityScale = originalGravity;
            isDashing = false;

            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }
    }   
}

