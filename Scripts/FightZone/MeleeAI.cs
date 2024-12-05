using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class MeleeAI : BaseAIController
{
   
    public override void Disable()
    {
        Destroy(GetComponent<RigBuilder>());
        base.Disable();
    }

    private const string ATTACK_TRIGGER = "Hit";
    private bool attackAvailable = true;
    public override bool Attack()
    {
        if (!attackAvailable) return false;
        if (!base.Attack()) return false;
        StartCoroutine(AttackTimer());

        bool right = (CurrentTarget.position - transform.position).normalized.x > 0 ? true : false;
        MainAnimator.SetTrigger(ATTACK_TRIGGER + (right ? "_R" : "_L"));
        return true;
    }

    private IEnumerator AttackTimer()
    {
        attackAvailable = false;
        yield return new WaitForSeconds(Stats.attackDelay);
        attackAvailable = true;
    }
}
