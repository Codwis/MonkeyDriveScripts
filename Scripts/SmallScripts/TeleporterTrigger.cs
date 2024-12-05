using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterTrigger : MonoBehaviour
{
    [Tooltip("This is where player will be teleported to")]public Transform SpotToTeleport;
    [Tooltip("Changes the rotation to the spot transform")]public bool ChangeRotation = true;
    [Tooltip("Does the teleported thing keep the given axis as same")]public bool KeepX = false, KeepY = false, KeepZ = false;
    [Tooltip("Should it be player only if false player cant enter")]public bool PlayerOnly = true;
    public bool FloatsStopTP = false;
    public bool StopVelocity = false;

    private void OnTriggerEnter(Collider other)
    {
        //Check if its player and do according to the variables
        if (PlayerOnly)
        {
            CarController_CW cont = other.transform.root.GetComponentInChildren<CarController_CW>();
            if (!cont)
            {
                return;
            }

            if (cont.FloatAbilityActive() && FloatsStopTP) return;

            cont.Rb.velocity = Vector3.zero;
            cont.Rb.angularVelocity = Vector3.zero;

            SetPosRot(cont.transform);
            return;
        }
        else
        {
            if (other.transform.root.GetComponentInChildren<CarController_CW>())
            {
                return;
            }
        }

        SetPosRot(other.transform);

        void SetPosRot(Transform toSet)
        {
            //Rotation setting
            Quaternion rot = ChangeRotation ? SpotToTeleport.localRotation : toSet.rotation;

            //Spot setting
            Vector3 spot = SpotToTeleport.position;
            spot.x = KeepX ? toSet.position.x : spot.x;
            spot.y = KeepY ? toSet.position.y : spot.y;
            spot.z = KeepZ ? toSet.position.z : spot.z;

            //Applying the Changes
            toSet.SetPositionAndRotation(spot, rot);
        }
    }


}
