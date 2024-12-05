using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pickup : MonoBehaviour
{
    public Ability abilityToUnlock;
    public AudioClipPreset audioToPlay;
    public string textToSay = "";
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.GetComponentInChildren<MovementController>())
        {
            GameHandler.instance.PlayEffectAudio(audioToPlay);

            GameHandler.abilityToUnlockOnLoad = abilityToUnlock.GetType();
            TextWriter.instance.WriteText(textToSay, sceneToGo: 1);
        }
    }
}
