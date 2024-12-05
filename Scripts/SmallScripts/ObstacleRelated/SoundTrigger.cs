using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public AudioClipPreset audioToPlay;

    private bool _allow = true;
    private void OnTriggerEnter(Collider other)
    {
        if (!_allow) return;

        GameHandler.instance.PlayEffectAudio(audioToPlay);
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        _allow = false;
        yield return new WaitForSeconds(audioToPlay.Clip.length);
        _allow = true;
    }
}
