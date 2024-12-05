using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Tooltip("How fast does the bullet go")] public float bulletSpeed = 5;
    [Tooltip("How much damage does this do")] public float damage = 25;
    [Tooltip("gets played on collision will try get from self")] public ParticleSystem particlesOnCollision;
    [Tooltip("gets played on collision will try get from self")] public AudioSource audioToPlay;

    private Vector3 _moveDirection = Vector3.forward;
    private Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if(audioToPlay == null) audioToPlay = GetComponent<AudioSource>();
        if(particlesOnCollision == null) particlesOnCollision = GetComponent<ParticleSystem>();
    }

    private Transform _source;
    public void Setup(Vector3 direction,Transform source) //Set the direction called from turretAI
    {
        _moveDirection = direction;
        _source = source;
        StartCoroutine(AutoRemove());
    }

    private void Update() //Move the bullet
    {
        _rb.AddForce(bulletSpeed * Time.deltaTime * _moveDirection, ForceMode.Impulse);
    }

    private IEnumerator AutoRemove()
    {
        yield return new WaitForSeconds(30);
        gameObject.AddComponent<DestroyTimer>().secondsTillDestroy = 2.5f;
    }

    private bool alreadyCollided = false;
    private void OnCollisionEnter(Collision collision)
    {
        //Check if already collided or if its the same root
        if (alreadyCollided) return;
        if (_source.root == collision.collider.transform.root) return;

        alreadyCollided = true;

        //If there are particles play them and make sure it gets destroyed
        if (particlesOnCollision != null)
        {
            var t = particlesOnCollision.main;
            t.stopAction = ParticleSystemStopAction.Destroy;
            particlesOnCollision.Play();
        } 
        else gameObject.AddComponent<DestroyTimer>().secondsTillDestroy = 2.5f; //Else add simple destroy timer

        if (audioToPlay != null) audioToPlay.Play();

        //Hide the bullet
        GetComponent<MeshRenderer>().enabled = false;

        var staff = collision.collider.GetComponent<StaffScript>();
        if (staff != null)
        {
            _source = staff.transform;
            return;
        }

        //Deal damage
        var stats = collision.collider.transform.root.GetComponentInChildren<FightStats>();
        if (stats != null) stats.TakeDamage(damage);
    }
}
