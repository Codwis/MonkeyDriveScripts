using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Cosmetic Global")]
    public AudioClipPreset audioOnInteract;

    public virtual void Start()
    {

    }
    public virtual void Interact(Transform source) //Gets called when interacted with
    {
        if (audioOnInteract.Clip != null) GameHandler.instance.PlayEffectAudio(audioOnInteract);
    }
}
