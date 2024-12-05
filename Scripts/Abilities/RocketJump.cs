using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketJump : Ability
{
    public override void EnableAbility(bool loaded)
    {
        base.EnableAbility(loaded);
        CarController.EnableAbility(this);
    }

    public override void Use()
    {
        base.Use();
    }

    public override void Charge(bool on)
    {
        base.Charge(on);

    }
}
