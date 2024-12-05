using Codwis;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTrigger : Interactable
{
    [Header("Main")]
    [Tooltip("Which object does this require")]public HoldableObject ObjectNeeded;

    [Header("Optional")]
    [Tooltip("An Event to call if needed")] public Codwis.Event EventToInvoke;
    [Tooltip("Will this be gameobject be destroyed")] public bool DestroyOnCorrect;
    public AudioClipPreset AudioOnCorrect;
    public bool DestroyScriptOnCorrect = true;
    [Tooltip("The spot where the object will be placed")]public Transform PlaceSpot;

    public bool DisableColliders = false;
    public HoldableObject CurrentObject { get; private set; }
    public bool Correct { get; private set; } = false;

    private MovementController _player;


    public override void Start()
    {
        base.Start();
        _player = FindAnyObjectByType<MovementController>();
    }
    public override void Interact(Transform source)
    {
        base.Interact(source);
        if (CurrentObject != null)
        {
            source.GetComponent<MovementController>().Pickup(CurrentObject);
            Correct = false;

            if (DisableColliders)
            {
                foreach (var col in CurrentObject.GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                }
            }

            CurrentObject = null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_allow) return;
        
        if (collision.collider.TryGetComponent<HoldableObject>(out HoldableObject obj))
        {
            if (CurrentObject != null) return;

            CurrentObject = obj;
            if (PlaceSpot != null)
            {
                _player.Drop();
                CurrentObject.Place(PlaceSpot);
            }

            if (CurrentObject == ObjectNeeded)
            {
                Activate();
            }

            StartCoroutine(SmallDelay());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_allow) return;

        if (other.TryGetComponent<HoldableObject>(out HoldableObject obj))
        {
            if (CurrentObject != null) return;

            CurrentObject = obj;
            if (PlaceSpot != null)
            {
                _player.Drop();
                CurrentObject.Place(PlaceSpot);
            }


            if (CurrentObject == ObjectNeeded)
            {
                Activate();
            }

            StartCoroutine(SmallDelay());
        }
    }

    private bool _allow = true;

    private IEnumerator SmallDelay()
    {
        _allow = false;
        yield return new WaitForSeconds(1);
        _allow = true;
    }

    private void Activate()
    {
        Action ac = new(CallBack);
        Correct = true;
        if(DisableColliders)
        {
            foreach(var col in CurrentObject.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        if(AudioOnCorrect.Clip != null) GameHandler.instance.PlayEffectAudio(AudioOnCorrect);

        if (EventToInvoke != null) EventToInvoke.Invoke(ac);
    }

    public void CallBack()
    {
        if (DestroyOnCorrect) Destroy(ObjectNeeded.gameObject);
        else if(DestroyScriptOnCorrect) Destroy(this);
    }
}
