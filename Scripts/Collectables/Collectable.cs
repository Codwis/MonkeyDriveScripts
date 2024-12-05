using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public string CollectableId { get; set; }
    [Tooltip("The color of the particles probably doesnt work")]public Color ParticleColor = default;
    [Tooltip("Audio which playes when picked up")]public AudioClipPreset Audio;
    [Tooltip("Rarely needed if this unlocks some cosmetical like license")]public CosmeticEquipment CosmeticToUnlock;
    public Ability AbilityToUnlock;
    [Tooltip("Prefab of the collectable for menu")]public GameObject CollectablePrefab;
    public Sprite CollectableSprite;
    private bool _taken = false;
    private void Start()
    {
        ParticleColor = ParticleColor == default ? Color.yellow : ParticleColor;
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        //So cant be taken multiple times
        if (_taken) return;

        HandlePickup(other);
    }

    public bool DestroyEntireThing = false;
    //Handles picking up collectable and effects
    private void HandlePickup(Collider other) 
    {
        //Get player collectables script
        PlayerCollectables collectables = other.GetComponentInParent<PlayerCollectables>();
        if (collectables == null) return;
        _taken = true;

        //Play audio and particles
        if (Audio.Clip != null) GameHandler.instance.PlayEffectAudio(Audio);
        GameHandler.instance.PlayBananaParticle(transform, ParticleColor);

        //Add collectable
        collectables.AddCollectable(this, 1);

        //Hide the renderer then particle system destroys it with stop action
        foreach (var rend in GetComponentsInChildren<MeshRenderer>()) rend.enabled = false;

        if (CosmeticToUnlock != null) other.transform.root.GetComponentInChildren<PlayerUnlockables>().AddUnlockable(CosmeticToUnlock);
        if (AbilityToUnlock != null) other.transform.root.GetComponentInChildren<PlayerUnlockables>().AddAbility(AbilityToUnlock);

        Destroy(GetComponent<Spinner>());
        if (DestroyEntireThing) Destroy(gameObject);
        Destroy(this); //Collectible no longer needed
    }
}

[System.Serializable]
public class AudioClipPreset
{
    public AudioClip Clip;
    [Range(0.1f,3)]public float Pitch = 1;
    [Range(0.01f, 1)] public float Volume = 0.2f;
}
