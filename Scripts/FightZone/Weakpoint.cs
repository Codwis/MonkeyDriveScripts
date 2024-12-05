using UnityEngine;

public class Weakpoint : MonoBehaviour //Small script for the weakpoints
{
    private BaseAIController _main;
    public AudioClipPreset weakhitSound;
    private void Start()
    {
        _main = GetComponentInParent<BaseAIController>();
    }

    public void Hit()
    {
        WeakpointParticles.instance.PlayParticles(transform);
        _main.DestroyWeakpoint(this);
    }
    public void WeakHit()
    {
        GameHandler.instance.PlayEffectAudio(weakhitSound);
    }
}
