using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [Header("Door Related")]
    [Tooltip("Does this start closed or open")]public bool startClosed = true;
    [Tooltip("Does the code addd -90 or 90 Y euler")]public bool negative90 = false;
    [Tooltip("Does the Y need to be doubled to 180 or -180 if negative")]public bool doubleRot = false;

    private Quaternion _closedRot;
    private Quaternion _openRot;

    private bool _closed;
    public override void Start()
    {
        base.Start();

        _closed = startClosed; 
        if(startClosed) //Setup the script depending on state
        {
            Setup(ref _closedRot, ref _openRot);
        }
        else
        {
            Setup(ref _openRot, ref _closedRot);
        }
    }

    //Sets the rotations open and closed ones
    private void Setup(ref Quaternion startRot, ref Quaternion otherRot) 
    {
        startRot = transform.localRotation;
        float y = negative90 ? -90 : 90;
        y *= doubleRot ? 2 : 1;

        otherRot = Quaternion.Euler(startRot.eulerAngles + new Vector3(0, y, 0));
    }

    private bool _opening = false;
    public override void Interact(Transform source) //Getting interacted call
    {
        if (_opening) return;

        base.Interact(source);
        StartCoroutine(Opening(_closed ? _openRot : _closedRot));
    }

    private const float MAX_INDEX = 1;
    public IEnumerator Opening(Quaternion rot) //Does the Opening/Closing
    {
        //Set variables
        _opening = true;
        float currentIndex = 0;
        Quaternion initialRot = transform.localRotation;

        //Smoothly open/close the door using lerp just changing the localrotation
        while(currentIndex <= MAX_INDEX)
        {
            currentIndex += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(initialRot, rot, currentIndex / MAX_INDEX);
            yield return new WaitForEndOfFrame();
        }

        //Allow interaction again
        _closed = !_closed;
        _opening = false;
    }
}
