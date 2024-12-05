using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurretAI : BaseAIController
{
    [Header("Turret Related")]

    [Tooltip("This is shot")]public GameObject bulletPrefab;
    [Tooltip("This is where the bullet spawns and rotates to same angle")]public Transform bulletSpawnPoint;

    public override void Start()
    {
        base.Start();
        Stats.foldOnDeath = false;
        Stats.allowDamage = false;   
    }

    public override void Disable() //Gets called when it dies
    {
        Destroy(this);
    }

    private void Shoot() //Small method to create the bullet and set it up and restart coroutine
    {
        Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation, null).GetComponent<Bullet>().Setup(bulletSpawnPoint.forward, transform);

        if(CurrentTarget != null)
        {
            StartCoroutine(ShootTimer());
        }
    }

    private IEnumerator ShootTimer() //Small timer to delay the bullets
    {
        yield return new WaitForSeconds(Stats.attackDelay);
        Shoot();
    }

    public override void TargetChanged() //Gets called when target is found
    {
        base.TargetChanged();
        StartCoroutine(ShootTimer());
    }
    public override void TargetRemoved() //Gets called when target is removed
    {
        base.TargetRemoved();
        StopCoroutine(ShootTimer());
    }
}
