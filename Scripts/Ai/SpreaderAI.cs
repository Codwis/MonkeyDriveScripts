using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Europa
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpreaderAI : MonoBehaviour
    {
        private Rigidbody rb;

        private const float _movSpeed = 10;
        private void Start()
        {
            rb = GetComponent<Rigidbody>();

            rb.AddForce(-transform.up * _movSpeed, ForceMode.Impulse);
            rb.AddTorque(-transform.up * _movSpeed * 2, ForceMode.Impulse);
        }

        private void Update()
        {
            rb.AddForce(-transform.up * _movSpeed * Time.deltaTime / 2, ForceMode.Acceleration);
            rb.AddTorque(-transform.up * _movSpeed * Time.deltaTime, ForceMode.Acceleration);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    } 
}
