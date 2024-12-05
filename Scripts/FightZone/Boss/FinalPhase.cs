using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FinalPhase : BaseAIController
{
    [Header("Boss related")]
    public string[] textToSayOnStart;
    private int weakpointStartAmount;
    public static int DeathCount = 0;
    public override void Start()
    {
        base.Start();
        weakpointStartAmount = weakPoints.Count;

        foreach (var item in turrentSpawnPoints)
        {
            item.AddComponent<TurretSpawnPoint>();
        }
    }

    public void StartBattle()
    {
        FindAnyObjectByType<FightMovementController>().Allow = false;
        System.Action actionToStart = new (StartupBattle);
        DeathCount = Mathf.Clamp(DeathCount, 0, textToSayOnStart.Length - 1);
        TextWriter.instance.WriteText(textToSayOnStart[DeathCount], fade: false, callBack: actionToStart);
    }

    public void StartupBattle()
    {
        allowChasing = true;
        FindAnyObjectByType<FightMovementController>().Allow = true;
    }

    [NonSerialized] public bool dead = false;
    private bool attackAvailable = true;
    private Coroutine attackCd;
    public override bool Attack()
    {
        if (dead) return false;

        if (goingForMelee) return false;
        if (!attackAvailable) return false;
        if (!base.Attack()) return false;

        var t = DecideAttackStyle();

        switch(t)
        {
            case AttackStyles.Laser:
                LaserAttack();
                break;
            case AttackStyles.Melee:
                StartCoroutine(MeleeAttack());
                break;
        }

        attackCd = StartCoroutine(AttackTimer());
        return true;
    }
    private IEnumerator AttackTimer()
    {
        attackAvailable = false;
        yield return new WaitForSeconds(Stats.attackDelay * Mathf.Clamp(((float)weakPoints.Count / (float)weakpointStartAmount), 0.25f, 1));
        attackAvailable = true;
    }

    public float distanceForLaser;
    private AttackStyles DecideAttackStyle()
    {
        if(Vector3.Distance(transform.position, CurrentTarget.position) > distanceForLaser && _canLaser)
        {
            return AttackStyles.Laser;
        }

        return AttackStyles.Melee;
    }

    //Melee Attack
    private bool goingForMelee = false;
    public float meleeRange;
    private const string ATTACK_TRIGGER = "Hit";
    private IEnumerator MeleeAttack()
    {
        goingForMelee = true;
        _agent.SetDestination(CurrentTarget.transform.position);
        Debug.Log("Going for melee");
        while(Vector3.Distance(transform.position, CurrentTarget.position) > meleeRange)
        {
            _agent.SetDestination(CurrentTarget.transform.position);
            yield return null;
        }

        bool right = (CurrentTarget.position - transform.position).normalized.x > 0 ? true : false;
        MainAnimator.SetTrigger(ATTACK_TRIGGER + (right ? "_R" : "_L"));
        yield return new WaitForSeconds(1.4f);

        if (laserCd != null) StopCoroutine(laserCd);
        _canLaser = true;
        
        _agent.SetDestination(GetFarthest());
        yield return new WaitForSeconds(1.4f);
        goingForMelee = false;
    }

    public override void ChaseTarget()
    {
        if (MainAnimator != null && _agent != null)
        {
            Vector3 velocity = _agent.velocity;
            float speed = velocity.magnitude / _agent.speed;

            // Calculate the dot product to determine forward/backward movement
            float forwardValue = Vector3.Dot(transform.forward, velocity.normalized) * speed;

            MainAnimator.SetFloat(ANIM_FORWARD, forwardValue);
        }

        if (CurrentTarget == null || !allowChasing) return;

        Attack();
    }

    [Header("Laser")]
    public GameObject laserCharge;
    public float laserChargeTime;
    public float laserCooldown;

    private bool _canLaser = true;

    public AudioClipPreset laserChargeSound;
    public AudioClipPreset laserShootSound;
    public GameObject laser;
    public MultiAimConstraint handAim;
    public Transform handAimTarget;
    public Camera player;
    private Coroutine laserCd;

    private const string LASER_START = "Laser_Start";
    private const string LASER_END = "Laser_End";
    private void LaserAttack()
    {
        goingForMelee = true;
        allowChasing = false;
        GameHandler.instance.PlayEffectAudio(laserChargeSound);
        Debug.Log("Going for laser");

        _agent.SetDestination(transform.position);
        MainAnimator.SetTrigger(LASER_START);

        laserCharge.SetActive(true);
        StartCoroutine(FireLaser());
        IEnumerator FireLaser()
        {
            float index = 0;
            Vector3 aimStart = handAimTarget.position;

            //Get random offset depending how many destroyed
            float randX = UnityEngine.Random.Range(-1, 1);
            randX *= Mathf.Clamp(((float)weakPoints.Count / (float)weakpointStartAmount), 0.1f, 1);
            float randY = UnityEngine.Random.Range(-1, 1);
            randY *= randX *= Mathf.Clamp(((float)weakPoints.Count / (float)weakpointStartAmount), 0.1f, 1);
            Vector3 offset = new Vector3(randX, randY);

            while (index <= laserChargeTime)
            {
                index += Time.deltaTime;
                handAim.weight = Mathf.Lerp(0, 1, index / (laserChargeTime / 2));

                handAimTarget.position = Vector3.Lerp(aimStart, player.transform.position + offset, index / laserChargeTime);
                yield return null;
            }

            MainAnimator.SetTrigger(LASER_END);
            GameHandler.instance.PlayEffectAudio(laserShootSound);

            handAim.weight = 0;
            allowChasing = true;

            //Get the direction to the target using cross ///Massive
            Vector3 dir = (handAimTarget.position - laserCharge.transform.position).normalized;


            //Create laser and set it go up direction which is forward for it
            GameObject temp = Instantiate(laser, laserCharge.transform.position, laserCharge.transform.rotation);
            temp.GetComponent<Bullet>().Setup(dir, transform);

            laserCharge.SetActive(false);

            goingForMelee = false;
            laserCd = StartCoroutine(LaserTimer());
        }
        

        IEnumerator LaserTimer()
        {
            _canLaser = false;
            yield return new WaitForSeconds(laserCooldown * Mathf.Clamp( ((float)weakPoints.Count / (float)weakpointStartAmount), 0.25f, 1));
            _canLaser = true;
        }
    }


    [Header("Ads")]
    public GameObject meleeAdSummon;
    public GameObject turrentAdSummon;
    
    public Transform[] summonPoints;
    public Transform[] turrentSpawnPoints;

    public int baseAmount = 2;
    private int weakpointsDestroyed = 0;

    public Transform spawnPoint;
    public AudioSource spawnAudio;

    private List<GameObject> ads = new List<GameObject>();
    private void SpawnAds()
    {
        
        int amount = (baseAmount + weakpointsDestroyed) - ads.Count / 2;
        amount = Mathf.Clamp(amount, 0, summonPoints.Length);
        amount -= 1;
        if (dead) amount = 0;
        StartCoroutine(Spawner(amount));

        IEnumerator Spawner(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (dead) yield break;

                spawnAudio.Play();
                var temp = Instantiate(meleeAdSummon, spawnPoint.position, spawnPoint.rotation).GetComponent<AdSummon>();
                temp.ListToAdd(ref ads);
                temp.SetTargetPos(summonPoints[i].position);

                if (turrentSpawnPoints[i].GetComponent<TurretSpawnPoint>().occupied == null && i % 2 == 0)
                {
                    temp = Instantiate(turrentAdSummon, spawnPoint.position, spawnPoint.rotation).GetComponent<AdSummon>();
                    temp.ListToAdd(ref ads, turrentSpawnPoints[i].GetComponent<TurretSpawnPoint>());
                    temp.SetTargetPos(turrentSpawnPoints[i].position);
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }

    public AudioClipPreset tpSound;
    public string textToSayOnFirstWeakpoint;
    public string textToSayOnSecondAttemptWeakpoint;

    public override void DestroyWeakpoint(Weakpoint point)
    {
        weakpointAudio.Play();
        
        weakpointsDestroyed++;
        if(weakpointsDestroyed == 1 && DeathCount == 0)
        {
            TextWriter.instance.WriteText(textToSayOnFirstWeakpoint, fade: false);
        }
        else if(weakpointsDestroyed == 1)
        {
            TextWriter.instance.WriteText(textToSayOnSecondAttemptWeakpoint, fade: false);
        }

        weakPoints.Remove(point);
        Destroy(point.gameObject);
        CheckForWeaks();

        TeleportFarthest();
    }

    private void TeleportFarthest()
    {
        _agent.destination = GetFarthest();
        _agent.Warp(_agent.destination);

        if (laserCd != null) StopCoroutine(laserCd);
        _canLaser = true;
        if (attackCd != null) StopCoroutine(attackCd);
        attackAvailable = true;
        SpawnAds();
        GameHandler.instance.PlayEffectAudio(tpSound);
    }

    private Vector3 GetFarthest()
    {
        int i = 0;
        int farth = 0;
        float farthest = 0;
        foreach (var item in summonPoints)
        {
            float dist = Vector3.Distance(item.position, player.transform.position);
            if (dist > farthest)
            {
                farthest = dist;
                farth = i;
            }
            i++;
        }
        return summonPoints[farth].position;
    }

    [Header("End")]
    public string textToSayOnWin;
    public BlendShapeToMax changer;
    public float secondsAtEnd = 5;
    public override void Disable()
    {
        dead = true;
        ads.ForEach((obje) => { if (obje != null) obje.GetComponent<FightStats>().Die(); });

        TextWriter.instance.WriteText(textToSayOnWin, callBack: CallBack);
        
        void CallBack()
        {
            StartCoroutine(changer.Change(secondsAtEnd));
            gameObject.AddComponent<DestroyTimer>().secondsTillDestroy = secondsAtEnd;
        }
    }
}
public enum AttackStyles { Melee, Laser, Cannon, Spawn }
