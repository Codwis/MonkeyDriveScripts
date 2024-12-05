using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCallback : MonoBehaviour
{
    private Ability MainAbility;
    private void Awake()
    {
        MainAbility = GetComponentInParent<Ability>();
    }

    private void OnParticleSystemStopped()
    {
        MainAbility.CleanUp();
    }
}
