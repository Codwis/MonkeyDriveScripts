using System.Collections;
using UnityEngine;

public class AIGuardWeapon : MonoBehaviour
{
    private Animator _mainAnimator;
    private const string returnTrigger = "Return";
    private BaseAIController _baseController;

    [Header("Cosmetic")]
    private ParticleSystem _particles;
    private AudioSource _audioToPlay;

    private void Start()
    {
        _mainAnimator = GetComponentInParent<Animator>();
    }

    private bool allowHit = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!allowHit) return;
        _mainAnimator.SetTrigger(returnTrigger);

        _baseController.EnableWeaponL(0);
        _baseController.EnableWeaponR(0);

        #region Cosmetic
        if (_particles != null)
        {
            _particles.transform.position = transform.position;
            _particles.Play();
        }

        if (_audioToPlay != null) _audioToPlay.Play();
        #endregion

        if (other.GetComponent<StaffScript>())
        {
            StartCoroutine(SmallDelay());
            return;
        }

        var stats = other.GetComponentInParent<FightStats>(); //deals damage if there is stats
        if (stats != null)
        {
            stats.TakeDamage(_baseController.Stats.Damage);
        }

        StartCoroutine(SmallDelay());
        IEnumerator SmallDelay()
        {
            allowHit = false;
            yield return new WaitForSeconds(0.2f);
            allowHit = true;
        }
    }

    public void SetBaseController(BaseAIController contr)
    {
        _baseController = contr;

        if (gameObject.name.EndsWith('R'))
        {
            _baseController.rightHandWeapon = this;
        }
        else if (gameObject.name.EndsWith('L'))
        {
            _baseController.leftHandWeapon = this;
        }

        _particles = contr.particlesToPlay;
        _audioToPlay = contr.hitSound;
    }
}
