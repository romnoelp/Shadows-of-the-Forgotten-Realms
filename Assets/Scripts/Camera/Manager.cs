using UnityEngine;
using Cinemachine;
using System.Collections;
using System;

namespace romnoelp
{
    public class Manager : MonoBehaviour
    {
        public static Manager instance; 
        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;

        [Header("Controls for damping the Y-axis during jump/fall")]
        [SerializeField] private float fallPanAmount = .25f;
        [SerializeField] private float fallPanTime = .35f;
        public float fallSpeedYDampingChangeThreshold = -15f;
        public bool isLerpingYDamping {get; private set;}
        public bool LerpedFromPlayerFalling {get; set;}
        private Coroutine LerpYPan;
        private CinemachineFramingTransposer framingTransposer;
        private CinemachineVirtualCamera activeCamera;
        private float normalYPanAmount;
        
        private void Awake() {
            if (instance == null)
            {
                instance = this;
            }

            for (int i = 0; i < virtualCameras.Length; i++)
            {
                if (virtualCameras[i].enabled)
                {
                    activeCamera = virtualCameras[i];
                    framingTransposer = activeCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                }
            }
            normalYPanAmount = framingTransposer.m_YDamping;

        }

        public void LerpYDamping(bool isPlayerFalling)
        {
            LerpYPan = StartCoroutine(LerpYAction(isPlayerFalling));
        }

        private IEnumerator LerpYAction(bool isPlayerFalling)
        {
            isLerpingYDamping = true;
            float startDampAmount = framingTransposer.m_YDamping;
            float endDampAmount = 0f;

            if(isPlayerFalling)
            {
                endDampAmount = fallPanAmount;
                LerpedFromPlayerFalling = true;
            }
            else
            {
                endDampAmount = normalYPanAmount;
            }
            
            float elapsedTime = 0f;
            while (elapsedTime < fallPanTime)
            {
                elapsedTime = elapsedTime + Time.deltaTime;
                float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallPanTime));
                framingTransposer.m_YDamping = lerpedPanAmount;
                yield return null;
            }
            isLerpingYDamping = false;
        }
    }
}

