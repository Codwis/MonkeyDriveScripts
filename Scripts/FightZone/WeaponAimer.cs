using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WeaponAimer : MonoBehaviour
{
    public Transform aimSpot;

    public float torqueForce = 10;
    public float dampening = 0.95f;
    [NonSerialized] public float ogTorque;

    private Rigidbody _rb;
    private void Start()
    {
        ogTorque = torqueForce;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        AimWeaponWithTorque();
    }
    //private void AimWeapon() Old method
    //{
    //    Vector3 directionToAim = aimSpot.position - transform.position;

    //    Quaternion targetRotation = Quaternion.LookRotation(directionToAim);

    //    // Smoothly rotate the weapon toward the aim spot
    //    _rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime));
    //}

    private void AimWeaponWithTorque()
    {
        // Calculate direction to the aim spot
        Vector3 directionToAim = aimSpot.position - transform.position;
        directionToAim.Normalize();

        // Calculate the current forward direction of the weapon
        Vector3 forwardDirection = transform.forward;

        // Calculate the angle using Dot Product
        float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(forwardDirection, directionToAim), -1f, 1f)) * Mathf.Rad2Deg;

        // Calculate the rotation axis using Cross Product
        Vector3 rotationAxis = Vector3.Cross(forwardDirection, directionToAim);
        // Threshold for small angles
        if (angle < 1f)
        {
            // Stop rotation and clear angular velocity
            _rb.angularVelocity = Vector3.zero;
            return;
        }


        // Apply torque around the calculated axis
        Vector3 torque = rotationAxis.normalized * angle * torqueForce;
        _rb.AddTorque(torque, ForceMode.Force);

        // Dampen angular velocity to prevent overshooting
        _rb.angularVelocity *= dampening;

        // Stabilize Z-axis rotation
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
    }

    public float GetAngularVelocity()
    {
        return _rb.angularVelocity.magnitude;
    }
    public void ResetAngularVelocity()
    {
        _rb.angularVelocity = Vector3.zero;
    }
}

