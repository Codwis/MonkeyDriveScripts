using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ability : MonoBehaviour
{
    [Header("COSMETIC")]
    [Header("Particle Effects")]
    [Tooltip("This will be played during the charging of the ability")]public AbilityEffect[] ChargeEffects;
    [Tooltip("Main effects of the played when Use() is called leave blank for increase charge rate")]public ParticleSystem[] MainEffects;
    public GameObject UiObject;

    [Header("Audio")]
    public AudioClipPreset ChargeAudioEffect;
    public AudioClipPreset MainEffectAudio;

    [Header("Showcase")]
    [Tooltip("This will be shown in the showcase")] public GameObject ShowcasePrefab;
    [Tooltip("This will be added to showcase prefabs local position")]public Vector3 offset;
    [Tooltip("This will be showcase objects localscale")] public Vector3 ShowcaseLocalScale = Vector3.one;
    [Tooltip("The name of the input which uses this")]public string InputName;
    [Tooltip("Local Scale for the input image")]public Vector3 InputScale = Vector3.one;
    [Tooltip("How fast does it go updown in showcase default 0.01")]public float UpAmplitude = 0.01f;
    public string AbilityId { get; private set; }

    //CHANGING NAMING METHOD BACK TO ORIGINAL FUCK THE UPPERCASE SHIT
    public CarController_CW CarController { get; private set; }

    public virtual void Awake()
    {
        CarController = transform.root.GetComponentInChildren<CarController_CW>();
    }

    public void SetId(string id)
    {
        AbilityId = id;
    }

    public bool Active { get; private set; } = false;
    public virtual void EnableAbility(bool loaded) //Enables the ability
    {
        if(!loaded) UnlockablesHandler.instance.Showcase(this);
        CarController.EnableAbility(this);
        Active = true;

        foreach (MeshRenderer rend in GetComponentsInChildren<MeshRenderer>(true))
        {
            rend.gameObject.SetActive(true);
        }
        if (UiObject != null)
        {
            UiObject.SetActive(true);
        }
        else Debug.LogError("No Ui object for ability " + gameObject.name);
    }
    public virtual void Use() //This gets called after charging once or can be called anytime watch out for loops
    {
        if(MainEffectAudio.Clip != null)
        {
            GameHandler.instance.PlayEffectAudio(MainEffectAudio);
        }
        foreach(ParticleSystem sys in MainEffects) //Play main effects
        {
            if (sys != null) sys.Play();
        }

        foreach (AbilityEffect ef in ChargeEffects) //Increase the rate if needed
        {
            if (!ef.IncreaseInUse) return;

            if (ef.Main != null)
            {
                ParticleSystem.EmissionModule module = ef.Main.emission;
                module.rateOverTimeMultiplier = ef.MaxRateOverTimeMulti;
                var t = ef.Main.main;
                t.loop = false;
            }
        }
    }
    public virtual void UseLoop() //This gets called after charging once or can be called anytime watch out for loops
    {
        if (MainEffectAudio.Clip != null)
        {
            GameHandler.instance.PlayLoopEffectAudio(MainEffectAudio);
        }
        foreach (ParticleSystem sys in MainEffects) //Play main effects
        {
            var mod = sys.main;
            mod.loop = true;
            if (sys != null) sys.Play();
        }
    }
    public void StopLoop()
    {
        if (MainEffectAudio.Clip != null)
        {
            GameHandler.instance.StopLoop();
        }
        foreach (ParticleSystem sys in MainEffects) //Play main effects
        {
            var mod = sys.main;
            mod.loop = false;
            if (sys != null) sys.Stop();
        }
    }


    public void CleanUp()
    {
        foreach (AbilityEffect ef in ChargeEffects) 
        {
            if (ef.Main != null)
            {
                ParticleSystem.EmissionModule module = ef.Main.emission;
                module.rateOverTimeMultiplier = ef.MinRateOverTimeMulti;

                var t = ef.Main.main;
                t.loop = true;
                ef.Main.Stop();
            }
        }
    }

    public virtual void Charge(bool on) //Charges up the ability called once and after to disable
    {
        foreach(AbilityEffect ef in ChargeEffects) //Turns on the effects
        {
            if (ef.Main != null)
            {
                ef.Main.Play();
            }
        }
        if (ChargeAudioEffect != null)
        {
            GameHandler.instance.PlayLoopEffectAudio(ChargeAudioEffect);
        }
    }

    public void GreyOutUi(bool on) //This is to hide the ui or grey out later
    {
        UiObject.SetActive(on);
    }
    
}
[System.Serializable]
public class AbilityEffect
{
    public ParticleSystem Main;
    public bool IncreaseInUse;
    public float MaxRateOverTimeMulti;
    public float MinRateOverTimeMulti;
}

