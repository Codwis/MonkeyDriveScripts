using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStats : FightStats
{
    [Header("AI related")]
    [Tooltip("How far does the ai agro from")] public float agroDistance = 10;
    [Tooltip("This is for melee how close player needs to be to attack added to remaining distance")]public float attackRange = 1.5f;
    [Tooltip("Delay between attacks")]public float attackDelay = 10;
    [NonSerialized] public BaseAIController _controller;

    [NonSerialized] public Action actionToCallOnDeath;
    public override void Start()
    {
        base.Start();
        _controller = GetComponent<BaseAIController>();
    }

    public void DisableController() //Gets called when dies
    {
        _controller.Disable();
    }
    public override void Die()
    {
        base.Die();
    }

    private void OnDestroy()
    {
        actionToCallOnDeath?.Invoke();
    }
}
