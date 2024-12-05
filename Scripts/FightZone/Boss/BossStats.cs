using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : AIStats
{
    public float phase2Health;
    public override void Die()
    {
        _currentHealth = maxHealth;
        _controller.Disable();
    }
}
