using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossAI : BaseAIController
{
    [Header("Boss Related")]
    public string textToSayOnHostile;

    public string textToSayOnFriendly;
    private int _phase = 0;
    public bool Agressive { get; set; } = true;
    private Action actionToStart;

    public override void Start()
    {
        base.Start();
        allowChasing = false;
    }

    [SerializeField]
    public Codwis.Event chairEvent;
    public void StartBattle()
    {

        if(Agressive)
        {
            FindAnyObjectByType<FightMovementController>().Allow = false;
            actionToStart = new Action(HostilePath);
            MainAnimator.SetTrigger("Stand");
            chairEvent?.Invoke(NullSh);
            TextWriter.instance.WriteText(textToSayOnHostile, fade: false, callBack: actionToStart);
        }
        else
        {
            actionToStart = new Action(FriendlyPath);
            TextWriter.instance.WriteText(textToSayOnFriendly, fade: false, callBack: actionToStart);
        }
    }
    public void NullSh()
    {

    }

    public override void TargetChanged()
    {
        base.TargetChanged();
    }

    private void FriendlyPath()
    {

    }
    private void HostilePath()
    {
        FindAnyObjectByType<FightMovementController>().Allow = true;
        _phase = 1;
        AllowChase();
    }
    public void AllowChase() //Called from animation?
    {
        allowChasing = true;
    }

    //Phase 1
    private const string ATTACK_TRIGGER = "Hit";
    private bool attackAvailable = true;
    public override bool Attack()
    {
        if (!attackAvailable) return false;
        if (!base.Attack()) return false;
        if (_phase != 1)
        {
            Phase2();
            return false;
        }

        StartCoroutine(AttackTimer());

        bool right = (CurrentTarget.position - transform.position).normalized.x > 0 ? true : false;
        MainAnimator.SetTrigger(ATTACK_TRIGGER + (right ? "_R" : "_L"));
        return true;
    }

    public void Phase2()
    {

    }

    private IEnumerator AttackTimer()
    {
        attackAvailable = false;
        yield return new WaitForSeconds(Stats.attackDelay);
        attackAvailable = true;
    }

    public GameObject phase2TransitionObject;
    public override void Disable()
    {
        _phase++;
        allowChasing = false;
        StartCoroutine(RunToSpot());
    }

    public Transform runSpot;
    public IEnumerator RunToSpot()
    {
        _agent.SetDestination(runSpot.position);
        while(_agent.remainingDistance > 1)
        {
            _agent.SetDestination(runSpot.position);
            yield return null;
        }
        phase2TransitionObject.SetActive(true);
        SpawnAds();
    }

    [Header("Ads")]
    public GameObject meleeAdSummon;
    public Transform[] summonPoints;
    public int amountPhase1 = 2;
    public Transform spawnPoint;
    public AudioSource spawnAudio;

    private List<GameObject> ads = new List<GameObject>();
    private void SpawnAds()
    {
        if(_phase == 2)
        {
            adsKilled = 0;
            StartCoroutine(Spawner(amountPhase1));
        }

        IEnumerator Spawner(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                spawnAudio.Play();
                var temp = Instantiate(meleeAdSummon, spawnPoint.position, spawnPoint.rotation).GetComponent<AdSummon>();
                temp.ListToAdd(ref ads, AdKilled);
                temp.SetTargetPos(summonPoints[i].position);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private int adsKilled = 0;
    public void AdKilled()
    {
        adsKilled++;
        if(adsKilled >= ads.Count)
        {
            if (_phase == 2) StartPhase2();
        }
    }

    [Header("Phase transition")]
    public string textToSayOnPhase2;
    public GameObject teleportToFinalPhase;
    private void StartPhase2()
    {
        phase2TransitionObject.SetActive(false);
        Instantiate(teleportToFinalPhase, transform.position, teleportToFinalPhase.transform.rotation);
        TextWriter.instance.WriteText(textToSayOnPhase2, fade: false);

        Destroy(gameObject);
    }
}
