using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelMover : MonoBehaviour
{
    [Tooltip("Where and how much should it move")]public Vector3 Dir;
    [Tooltip("How much should it rotate")]public Vector3 Rot;
    [Tooltip("This is added to the bottom")]public float ExtraPadding;

    private bool _currentlyGoing = false;
    private LayerMask _selfMask;

    private void Start()
    {
        _selfMask = ~(1 << gameObject.layer);
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5, _selfMask))
        {
            transform.position = hit.point + Vector3.up * ExtraPadding;
        }
    }

    public void Activate(bool on, int takerLayer = 0) //Activates the barrel mover
    {
        if(on)
        {
            _selfMask = ~((1 << takerLayer) | (1 << gameObject.layer));
        }

        _currentlyGoing = on;
    }

    private void FixedUpdate()
    {
        if(_currentlyGoing) 
        {
            //Moves and rotates the barrel
            transform.Translate(Dir * Time.deltaTime, Space.World);
            transform.localEulerAngles += Rot;

            //Sets it to the ground and adds the padding
            if(Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5, _selfMask))
            {
                transform.position = hit.point + Vector3.up * ExtraPadding;
            }
        }
    }
}
