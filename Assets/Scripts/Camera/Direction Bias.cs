using System.Collections;
using UnityEngine;

namespace romnoelp
{
    public class DirectionBias : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        private bool isFacingRight;
        private Movement player;

        [Header("Camera Smoothing Values")]
        [SerializeField] private float yAxisRotationFlipTime = .5f;
        private Coroutine LerpYAxis;

        private void Awake() {
            player = playerTransform.gameObject.GetComponent<Movement>();
            isFacingRight = player.isFacingRight;
        }

        void Update()
        {
            transform.position = playerTransform.position;
        }

        public void CallTurn()
        {
            // Installed a 3rd party library called LeanTween to handle the smoothing of switch 
            // direction of camera
            LeanTween.rotateY(gameObject, DetermineEndRotation(), yAxisRotationFlipTime).setEaseInOutSine();

            // If line 31 gives you an error in your unity editor, comment that and uncomment line 34, 
            // see if it will change anything for your case.
            // LerpYAxis = StartCoroutine(FlipYAxis());
        }

        private IEnumerator FlipYAxis()
        {
            float startRotation = transform.localEulerAngles.y;
            float endRotationAmount = DetermineEndRotation();
            float yRotation;

            float elapsedTime = 0f;

            while (elapsedTime < yAxisRotationFlipTime)
            {
                elapsedTime = elapsedTime + Time.deltaTime;
                yRotation = Mathf.Lerp(startRotation, endRotationAmount, elapsedTime / yAxisRotationFlipTime);   
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
            yield return null;
        }

        private float DetermineEndRotation()
        {
            isFacingRight = !isFacingRight;
            if (isFacingRight)
            {
                return 0f;
            }
            else
            {
                return 180f;
            }
        }
    }
}