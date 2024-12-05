using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AIStats))]
public class BaseAIController : MonoBehaviour
{
    [Tooltip("Layers which get the ai targets")]public LayerMask targetMask;
    [Range(0,100), Tooltip("What is the chance for each weakpoint to activate")]public int weakpointChance;

    [Header("Melee")]
    [NonSerialized]public AIGuardWeapon leftHandWeapon;
    [NonSerialized]public AIGuardWeapon rightHandWeapon;
    public ParticleSystem particlesToPlay;
    public AudioSource hitSound;

    [Header("Optional")]
    [Tooltip("This gets triggered on target found and lost changes blend shape")]public ApproachTrigger approach;
    public List<Weakpoint> weakPoints { get; set; } = new List<Weakpoint>();
    public AIStats Stats { get; private set; }
    public Animator MainAnimator { get; private set; }

    [NonSerialized] public NavMeshAgent _agent;
    private LayerMask _selfMask;

    public const string ANIM_FORWARD = "Axis_Y";
   
    public virtual void Start()
    {
        Stats = GetComponent<AIStats>();
        _agent = GetComponent<NavMeshAgent>();
        _selfMask = ~((1 << gameObject.layer) | targetMask);

        //Setup weapons
        AIGuardWeapon[] weps = GetComponentsInChildren<AIGuardWeapon>();
        foreach (var item in weps)
        {
            item.SetBaseController(this);
        }

        MainAnimator = GetComponent<Animator>();
        if(MainAnimator != null && this is not BossAI) MainAnimator.enabled = false;
        if (approach == null) approach = GetComponentInChildren<ApproachTrigger>();

        if(_agent != null) _agent.speed = Stats.movSpeed;

        #region WeakpointSetting
        bool atleastOne = false;
        int count = GetComponentsInChildren<Weakpoint>(true).Length;
        int counter = 0;
        foreach (var item in GetComponentsInChildren<Weakpoint>(true))
        {
            if(this is FinalPhase)
            {
                weakPoints.Add(item);
                continue;
            }
            counter++;

            int rand = UnityEngine.Random.Range(0, 100);
            if(rand <= weakpointChance || counter == count && !atleastOne) //Activates the weakpoint if random hits or last one if none has
            {
                item.gameObject.SetActive(true);
                weakPoints.Add(item);
                atleastOne = true;
            }
            else
            {
                Destroy(item.gameObject);
            }
        }
        #endregion

        targetChanged = TargetChanged;
        targetRemoved = TargetRemoved;
    }

    private void Update()
    {
        if (this is FinalPhase t) if (t.dead) return;

        CheckForTargets();
        ChaseTarget();
        TurnTowardsTarget(); // turn towards target
    }

    #region Target_Property
    public Action targetChanged;
    public Action targetRemoved;
    private Transform _currentTarget;
    public Transform CurrentTarget //First property with custom set
    {
        get => _currentTarget;
        set
        {
            if (value == null)
            {
                if(_currentTarget != null)
                {
                    Action action = new Action(() => allowChasing = false);
                    if (approach != null) approach.ToStart(ref action);

                    _currentTarget = null;
                    targetRemoved?.Invoke();
                }
            }
            else if (_currentTarget != value)
            {
                Action action = new Action(() => allowChasing = true);
                if (approach != null) approach.ToEnd(ref action);

                _currentTarget = value;
                targetChanged?.Invoke();
            }
        }
    }
    public virtual void TargetChanged() //Gets called when target gets changed
    {
        if(MainAnimator != null) MainAnimator.enabled = true;
    }
    public virtual void TargetRemoved() //Gets called when target becomes null
    {
       // if (MainAnimator != null) MainAnimator.enabled = false;
    }
    #endregion

    public bool Allow { get; set; } = true;
    private void CheckForTargets() //Checks for targets wiht a sphere cast and checks which is closest and sets target
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, Stats.agroDistance, Vector3.one, Stats.agroDistance, targetMask);
        if (hits != null)
        {
            Transform closest = null;
            foreach (var item in hits)
            {
                if (RaycastTarget(item.collider.transform)) continue;

                if (closest == null)
                {
                    closest = item.collider.transform.root;
                    continue;
                }
                if (closest == item.transform.root) continue;
                

                if (Vector3.Distance(transform.position, item.transform.root.position) < Vector3.Distance(transform.position, closest.position))
                {
                    closest = item.collider.transform.root;
                }
            }

            //For player
            if(closest != null)
            {
                var player = closest.root.GetComponentInChildren<FightMovementController>();
                if (player != null) closest = player.transform;
            }

            CurrentTarget = closest;
        }
        else CurrentTarget = null;

        if(this is FinalPhase t)
        {
            CurrentTarget = t.player.transform.root.GetComponentInChildren<FightMovementController>().transform;
        }
        bool RaycastTarget(Transform trans)
        {
            //ONLY Player detection for now
            if(Physics.Linecast(transform.position, trans.position, _selfMask))
            {
                return true;
            }
            return false;
        }
    }

    [Tooltip("allow instant chasing?")]public bool allowChasing = false;
    public virtual void ChaseTarget() //Starts chase towards target
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

        if (this is FinalPhase)
        {
            Attack();
            return;
        }

        if (_agent != null)  //Only chase if there is an agent
        {
            _agent.SetDestination(CurrentTarget.position);
            Attack();
        }

    }

    private void TurnTowardsTarget() //Turns the ai towards the target smoothly
    {
        if (CurrentTarget != null)
        {
            Vector3 dir = (CurrentTarget.position - transform.position).normalized;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5);
        }
    }

    public virtual bool Attack()
    {
        float distance = Vector3.Distance(transform.position, CurrentTarget.position);
        if(distance >= _agent.stoppingDistance + Stats.attackRange || CurrentTarget == null)
        {
            return false;
        }
        return true;
    }

    public virtual void Disable() //Gets called when this dies
    {
        if(_agent != null) _agent.enabled = false;
        if (leftHandWeapon != null) Destroy(leftHandWeapon);
        if (rightHandWeapon != null) Destroy(rightHandWeapon);
        if (MainAnimator != null) Destroy(MainAnimator);
        Destroy(this);
    }

    public AudioSource weakpointAudio;
    public virtual void DestroyWeakpoint(Weakpoint point) //Gets called when weakpoint is hit with a weapon
    {
        weakpointAudio.Play();
        weakPoints.Remove(point);
        Destroy(point.gameObject);

        CheckForWeaks();
    }
    public virtual void CheckForWeaks() //Simple just checks if there are any weakpoints left or not if not die
    {
        if (weakPoints.Count == 0)
        {
            Stats.Die();
        }
    }

    //Animations Events
    public void EnableWeaponL(int on = 0)
    {
        leftHandWeapon.GetComponent<Collider>().isTrigger = on == 0 ? false : true;
    }
    public void EnableWeaponR(int on = 0)
    {
        rightHandWeapon.GetComponent<Collider>().isTrigger = on == 0 ? false : true;
    }
}
