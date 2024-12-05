using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossStartEvent : Codwis.Event
{
    public UnityEvent onDoorClose;

    public override void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<StaffScript>() || !other.transform.root.GetComponentInChildren<FightMovementController>()) return;

        if (activated) return;
        activated = true;
        Invoke(CallBack);
    }

    public void CallBack()
    {
        onDoorClose.Invoke();
        Destroy(gameObject);
    }
}
