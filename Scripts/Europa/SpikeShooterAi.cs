using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    public class SpikeShooterAi : MonoBehaviour
    {
        [Header("Follow")]
        [Tooltip("Target which the spike follows")] public Transform spikeTarget;
        [Tooltip("Spike tip which will follow the Target")] public Transform parentWhichFollows;

        private LayerMask targetMask;
        private Vector3 ogSpot;
        private Transform spot;

        private void Start()
        {
            targetMask = ~(1 << gameObject.layer);
            ogSpot = spikeTarget.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            //sets the target to the alive thing
            PlayerController controller = other.transform.root.GetComponentInChildren<PlayerController>();
            if (controller != null)
            {
                spot = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            //Removes spot only if it was the one that triggered it
            PlayerController controller = other.transform.root.GetComponentInChildren<PlayerController>();
            if (controller != null)
            {
                if (spot == other.transform)
                {
                    spot = null;
                }
            }
        }

        private void Update()
        {
            //Moves the targe to spot otherwise just back to origin
            if (spot != null)
            {
                spikeTarget.transform.position = Vector3.Lerp(spikeTarget.position, spot.position, 10 * Time.deltaTime);
            }
            else
            {
                spikeTarget.transform.position = Vector3.Lerp(spikeTarget.position, ogSpot, 10 * Time.deltaTime);
            }

            //Makes the tip follow the target
            parentWhichFollows.position = Vector3.Lerp(parentWhichFollows.position, spikeTarget.position, 13 * Time.deltaTime);
            parentWhichFollows.rotation = Quaternion.Lerp(parentWhichFollows.rotation, spikeTarget.rotation, 13 * Time.deltaTime);
        }
    } 
}
