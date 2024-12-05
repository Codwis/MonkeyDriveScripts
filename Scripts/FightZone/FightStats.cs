using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightStats : MonoBehaviour
{
    [Header("Main")]
    [Tooltip("How fast does it move")]public float movSpeed = 10;
    [Tooltip("Maximum health")]public float maxHealth = 100;
    public float Damage = 20;

    [Tooltip("Is damage allowed")] public bool allowDamage = true;
    [Tooltip("Do rigidbodies constraints set to none")]public bool foldOnDeath = true;
    [NonSerialized] public float _currentHealth;

    [Header("UI")]
    public Slider healthBar;
    public virtual void Start()
    {
        _currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = _currentHealth;
        }
    }

    private bool _damageCooldownOn = false;
    private const float INVICIBILITY_SECONDS = 1;
    public virtual void TakeDamage(float amount) //Takes damage and checks if dead
    {
        if (!allowDamage) return;
        if (_damageCooldownOn) return;

        if(TryGetComponent<FightMovementController>(out var t))
        {
            t.TakeDamage();
        }
        _currentHealth -= amount;
        if (healthBar != null) healthBar.value = _currentHealth;
        if(_currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvicibilityTimer()); //Invicibility frames
        IEnumerator InvicibilityTimer()
        {
            _damageCooldownOn = true;
            yield return new WaitForSeconds(INVICIBILITY_SECONDS);
            _damageCooldownOn = false;
        }
    }

    private const float DESTROY_TIME = 4;
    [Tooltip("Does it get made small when dead")]public bool makeSmallOnDeath = true;
    private bool dead = false;
    public virtual void Die() 
    {
        if (dead) return;
        dead = true;
        if(foldOnDeath) //Disable constraitns on rigidbodies
        {
            foreach (var item in transform.root.GetComponentsInChildren<Rigidbody>())
            {
                item.constraints = RigidbodyConstraints.None;
            }
        }

        if(this is AIStats st) //If its ai add destroy timer
        {
            st.DisableController();
            if(makeSmallOnDeath) gameObject.AddComponent<DestroyTimer>().secondsTillDestroy = DESTROY_TIME;
        }
        
        if(TryGetComponent<FightMovementController>(out FightMovementController contr))
        {
            contr.OnDeath();
            return;
        }

        Destroy(this);
    }
}
