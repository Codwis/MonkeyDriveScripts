using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerAbility : Ability
{
    [Header("Scanner")]
    [Tooltip("The Cooldown how often this can be used")]public float cooldown = 15;
    [Tooltip("The main gameobject which grows and has ScannerBubble.cs")]public GameObject scanBubble;

    public override void Use()
    {
        base.Use(); //Noise and Particles
        Instantiate(scanBubble, transform.position, transform.rotation);
    }
}
