using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldableObject : Interactable
{
    [Tooltip("This is the spot where Players hand Ik right goes")] public Transform HandSpotIK_R;
    [Tooltip("This is the spot where Players hand Ik left goes")]public Transform HandSpotIK_L;
    public Vector3 ExtraEuler;
    private Rigidbody _rb;
    private Vector3 _ogEulers;

    private KnockingWhispering _knocks;
    public override void Start()
    {
        base.Start();
        _knocks = FindAnyObjectByType<KnockingWhispering>();
        _ogEulers = Vector3.zero;
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_current != null)
        {
            transform.position = Vector3.Lerp(transform.position, _current.position, 5 * Time.deltaTime);
            Quaternion rot = Quaternion.Euler(_ogEulers + ExtraEuler);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, 5 * Time.deltaTime);

            if(_right != null && _left != null)
            {
                _right.SetPositionAndRotation(HandSpotIK_R.position, HandSpotIK_R.rotation);
                _left.SetPositionAndRotation(HandSpotIK_L.position, HandSpotIK_L.rotation);
            }
        }
    }

    public override void Interact(Transform source) //Gets called when interacted with
    {
        base.Interact(source);
        if (_knocks != null && transform.root.name == "MonkeRagdoll") _knocks.StartWhispers();

        source.GetComponent<MovementController>().Pickup(this);
    }

    private Transform _current;
    private Transform _right, _left;
    public void Pickup(ref Transform right, ref Transform left, Transform spot) //Picks up the object
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _current = spot;
        transform.parent = spot;

        if(right != null && left != null)
        {
            _right = right;
            _left = left;
        }
    }
    public void Place(Transform spot)
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _current = spot;
        transform.parent = spot;
    }

    public void Drop() //Drops the object
    {
        foreach(var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
        }
        transform.parent = null;

        _right = null;
        _left = null;
        _current = null;
    }

    public virtual void Use()
    {

    }
}
