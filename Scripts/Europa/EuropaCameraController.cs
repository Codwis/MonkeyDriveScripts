using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class EuropaCameraController : MonoBehaviour
    {

        [Header("Checking For Objects")]
        [Tooltip("This should be everything except player, event, collectable")] public LayerMask MaskForCamera;
        private Vector3 _originSpot;
        [Tooltip("The point which camera rotates around")] public Transform PivotPoint;
        private float _originDistance;
        private void CheckForObjects()
        {
            // Ray from the player to the camera
            Ray ray = new Ray(PivotPoint.transform.position, (transform.position - PivotPoint.transform.position).normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, _originDistance, MaskForCamera))
            {
                // If the ray hits an object move the camera closer to the hit point
                float hitDistance = Vector3.Distance(PivotPoint.transform.position, hit.point);
                hitDistance = Mathf.Clamp(hitDistance, 0.1f, 10); //Clamp it so wont go -

                Vector3 dir = (transform.position - PivotPoint.transform.position).normalized; //Get direction
                transform.position = PivotPoint.transform.position + dir * hitDistance;  //Update position according to distance
            }
            else
            {
                //Otherwise just put it to original spot
                transform.localPosition = _originSpot;
            }
        }

        void Start()
        {
            _originDistance = Vector3.Distance(PivotPoint.transform.position, transform.position);
            _originSpot = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            CheckForObjects();
        }
    } 
}
