using System.Collections.Generic;
using JetBrains.Annotations;
using romnoelp;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class RopeSystem : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private DistanceJoint2D distanceJoint;

    void Start()
    {
        distanceJoint.enabled =false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            var ColliderHit = false;
            if(ColliderHit=true){
            Vector2 mousePos = (Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.SetPosition(0, mousePos);
            lineRenderer.SetPosition(10, transform.position);
            distanceJoint.connectedAnchor = mousePos;
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
            }
           
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
        }
        if (distanceJoint.enabled)
        {
            lineRenderer.SetPosition(1,transform.position);
        }
    }
    

}
 