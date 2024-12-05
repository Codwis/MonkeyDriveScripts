using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingItem : HoldableObject
{
    [Header("Swing Item")]
    public AudioClipPreset audioToPlay;
    public AudioClipPreset hitSound;

    public Animator animator;
    public string swingTrigger;
    public override void Use()
    {
        if (!_canSwing) return;

        base.Use();
        _canSwing = false;
        if (audioToPlay.Clip != null) GameHandler.instance.PlayEffectAudio(audioToPlay);
        animator.SetTrigger(swingTrigger);
    }

    private bool _canSwing = true;
    public void AllowSwing()
    {
        animator.ResetTrigger(swingTrigger);
        _canSwing = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root == transform.root) return;

        if (hitSound.Clip != null) GameHandler.instance.PlayEffectAudio(hitSound);
        var t = other.GetComponentInParent<BreakableWall>();
        if (t != null) t.Break(this);
    }
}
