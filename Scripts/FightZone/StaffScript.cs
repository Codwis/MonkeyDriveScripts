using System.Collections;
using UnityEngine;

public class StaffScript : MonoBehaviour
{
    [Tooltip("The main controller for player")] public FightMovementController mainController;
    [Tooltip("The script which handles the aiming")] public WeaponAimer aimer;
    [Tooltip("How much damage does it do")] public float damage = 20;
    public float knockbackIntensity = 10;

    [Header("Cosmetic")]
    [Tooltip("These play on collision point")] public ParticleSystem particles;
    [Tooltip("These play on collision point")] public AudioSource audioToPlay;
    public AudioClip customAudioOnBlock;
    public AudioClip customAudioOnStatsHit;
    public DuelMusicHandler duelMusic;

    private AudioClip _ogClip;
    private Transform current = null;
    private bool allowHit = true;
    private float audioLevelOg;
    private Vector3 particleOg;
    private void Start()
    {
        _ogClip = audioToPlay.clip;
        particleOg = particles.transform.localScale;
        audioLevelOg = audioToPlay.volume;
        duelMusic = transform.root.GetComponentInChildren<DuelMusicHandler>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root == transform.root) return;
        if (!allowHit) return;
        if (current == collision.collider.transform) return;
        if (aimer.GetAngularVelocity() > 7) return;
        current = collision.collider.transform;

        

        #region Cosmetic
        if (particles != null)
        {
            var point = collision.contacts[0];
            Quaternion rot = Quaternion.LookRotation(point.normal);
            particles.transform.SetPositionAndRotation(point.point, rot);

            //Change size of particles dynamically
            float val = aimer.GetAngularVelocity() / 3;
            val = Mathf.Clamp01(val);
            Vector3 value = Vector3.Lerp(Vector3.zero, particleOg, val);
            particles.transform.localScale = value;

            particles.Play();
        }

        #endregion

        //Change volume dynamically
        float vol = aimer.GetAngularVelocity() / 3;
        vol = Mathf.Clamp01(vol);
        if (duelMusic != null) duelMusic.HitStart(vol);
        vol = Mathf.Lerp(0, audioLevelOg, vol);
        audioToPlay.volume = vol;


        if (collision.collider.GetComponent<AIGuardWeapon>())
        {
            audioToPlay.clip = customAudioOnBlock;
            audioToPlay.Play();

            StartCoroutine(SmallDelay());
            return;
        }

        var stats = collision.collider.GetComponentInParent<FightStats>(); //deals damage if there is stats
        if (stats != null)
        {
            audioToPlay.clip = customAudioOnStatsHit;
            audioToPlay.Play();
            stats.TakeDamage(mainController._stats.Damage * aimer.GetAngularVelocity());
        }
        else
        {
            audioToPlay.clip = _ogClip;
            audioToPlay.Play();
        }
        if (collision.collider.TryGetComponent<Weakpoint>(out var weak)) //Destroyes weakpoint if there is one
        {
            if(aimer.GetAngularVelocity() > 0.2f)
            {
                weak.Hit();
            }
            else
            {
                weak.WeakHit();
            }
            
        }

        aimer.torqueForce = 1f;

        StartCoroutine(SmallDelay());

        IEnumerator SmallDelay()
        {
            allowHit = false;
            yield return new WaitForSeconds(0.2f);
            allowHit = true;
        }
    }

    private Coroutine waitTimer;
    private void OnCollisionExit(Collision collision)
    {
        if(waitTimer != null)
        {
            StopCoroutine(waitTimer);
        }
        waitTimer = StartCoroutine(WaitSecond());
    }
    private IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(0.2f);
        aimer.torqueForce = aimer.ogTorque;
        current = null;
    }
}
