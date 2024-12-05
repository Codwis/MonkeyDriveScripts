using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : MonoBehaviour
{
    [Tooltip("This will be shown in the showcase")]public GameObject ShowcasePrefab;
    [Tooltip("This is added in showcase to localposition")] public Vector3 offset;
    [Tooltip("Local Scale for showcase prefab")] public Vector3 ShowcaseLocalScale = Vector3.one;
    [Tooltip("How fast does it go updown in showcase default 0.01")] public float UpAmplitude = 0.01f;
    public bool Active { get; private set; } = false;
    public string UnlockableId { get; private set; }

    public void SetId(string id) //Set an ID
    {
        UnlockableId = id;
    }

    public virtual void Activate(bool on) //Activates the renderers and the unlockable
    {
        foreach (var rend in GetComponentsInChildren<Renderer>(true)) rend.enabled = true;
        foreach (var rend in GetComponentsInChildren<Renderer>(true)) rend.gameObject.SetActive(true);
        Active = on;
    }

    public virtual void Unlock(bool loaded) //Unlocks the unlockable and showcases if not loaded
    {
        if(!loaded) UnlockablesHandler.instance.Showcase(this);

        Activate(true);
    }
}
