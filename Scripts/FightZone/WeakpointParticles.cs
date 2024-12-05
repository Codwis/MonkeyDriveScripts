using UnityEngine;

public class WeakpointParticles : MonoBehaviour
{
    public static WeakpointParticles instance;
    private void Awake()
    {
        instance = this;
    }

    private ParticleSystem _particles;
    private void Start()
    {
        _particles = GetComponent<ParticleSystem>();
    }
    public void PlayParticles(Transform spot)
    {
        _particles.transform.SetPositionAndRotation(spot.position, spot.rotation);
        _particles.Play();
    }
}
