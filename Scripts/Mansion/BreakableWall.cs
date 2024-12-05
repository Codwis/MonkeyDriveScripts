using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public HoldableObject objectNeeded;
    public float destructiveAmplitude = 1;
    private Rigidbody[] rigidBodies;
    private void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
    }

    public void Break(HoldableObject obj)
    {
        if(obj.GetType() == objectNeeded.GetType())
        {
            foreach (var item in rigidBodies)
            {
                item.constraints = RigidbodyConstraints.None;
                item.AddForce(transform.position - obj.transform.position * destructiveAmplitude, ForceMode.Impulse);
                item.GetComponent<DestroyTimer>().StartCoroutine(item.GetComponent<DestroyTimer>().Destroy());
            }

            Destroy(this);
        }
    }
}
