using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockablesHandler : MonoBehaviour
{
    public static UnlockablesHandler instance;
    [Header("Showcase")]
    public Camera ShowcaseCam;
    public Transform ShowcaseParent;
    public Image ShowcaseInputImage;
    public GameObject ShowcaseCanvas;
    public AudioSource unlockSound;
    public ParticleSystem showcaseParticles;

    [Header("For Collection")]
    public Transform AbilityParent;
    public Transform UnlockableParent;
    public Ability[] Abilities { get; private set; }
    public Unlockable[] Unlockables { get; private set; }

    private CarController_CW _player;
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;


        _player = FindAnyObjectByType<CarController_CW>();
        AbilityParent = _player.GetComponentInChildren<ParentFind>().AbilityParent;
        UnlockableParent = _player.GetComponentInChildren<ParentFind>().CosmeticParent;
        int i;

        Abilities = AbilityParent ? AbilityParent.GetComponentsInChildren<Ability>(true) : null;
        Unlockables = UnlockableParent ? UnlockableParent.GetComponentsInChildren<Unlockable>(true): null;

        if (Abilities != null)
        {
            for (i = 0; i < Abilities.Length; i++)
            {
                Abilities[i].SetId(i.ToString());
            } 
        }
        if (Unlockables != null)
        {
            for (i = 0; i < Unlockables.Length; i++)
            {
                Unlockables[i].SetId(i.ToString());
            } 
        }
    }

    public Ability GetAbility(string id)
    {
        for(int i = 0; i < Abilities.Length; i++)
        {
            if (Abilities[i].AbilityId == id)
            {
                return Abilities[i];
            }
        }
        Debug.Log("Couldnt find Ability");
        return null;
    }
    public Ability GetAbility(Type type)
    {
        for (int i = 0; i < Abilities.Length; i++)
        {
            if (Abilities[i].GetType() == type)
            {
                return Abilities[i];
            }
        }
        Debug.Log("Failed to find");
        return null;
    }

    public Unlockable GetUnlockable(string id)
    {
        for (int i = 0; i < Unlockables.Length; i++)
        {
            if (Unlockables[i].UnlockableId == id)
            {
                return Unlockables[i];
            }
        }
        Debug.Log("Couldnt find Unlockable");
        return null;
    }

    public const float SHOWCASETIME = 3;
    public void Showcase(Unlockable unlock)
    {
        var temp = Instantiate(unlock.ShowcasePrefab, ShowcaseParent);
        temp.transform.localPosition += unlock.offset;
        temp.transform.localScale = unlock.ShowcaseLocalScale;

        var renderers = temp.GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in renderers)
        {
            rend.enabled = true;
        }

        var spin = temp.AddComponent<Spinner>();
        spin.UpAmplitude = unlock.UpAmplitude;
        spin.SpinAmount = Vector3.up * 100;

        ShowcaseInputImage.color = Color.clear;

        ShowcaseCam.gameObject.SetActive(true);
        CosmeticsShowcase();

        _player.Rb.velocity = Vector3.zero;
        _player.Rb.angularVelocity = Vector3.zero;

        StartCoroutine(StopShowcase());
        IEnumerator StopShowcase()
        {
            yield return new WaitForSeconds(SHOWCASETIME);
            ShowcaseCam.gameObject.SetActive(false);
            Destroy(temp);
        }
    }
    public void Showcase(Ability ability)
    {
        var temp = Instantiate(ability.ShowcasePrefab, ShowcaseParent);
        var renderers = temp.GetComponentsInChildren<MeshRenderer>(true);
        foreach(var rend in renderers)
        {
            rend.enabled = true;
            rend.gameObject.SetActive(true);
        }

        temp.transform.localPosition = Vector3.zero;
        temp.transform.localPosition += ability.offset;
        temp.transform.localScale = ability.ShowcaseLocalScale;

        var spin = temp.AddComponent<Spinner>();
        spin.UpAmplitude = ability.UpAmplitude;
        spin.UpFrequency = 2;
        spin.SpinAmount = Vector3.up * 100;

        if (ability.InputName != "")
        {
            var sprite = InputHandler.GetInputSprite(ability.InputName);
            ShowcaseInputImage.sprite = sprite ? sprite : null;
            ShowcaseInputImage.color = Color.white;
        }
        else 
        {
            ShowcaseInputImage.color = Color.clear;
        }
            
        ShowcaseInputImage.transform.localScale = ability.InputScale;
        ShowcaseCanvas.SetActive(true);
        ShowcaseCam.gameObject.SetActive(true);

        CosmeticsShowcase();

        StartCoroutine(StopShowcase());
        IEnumerator StopShowcase()
        {
            yield return new WaitForSeconds(SHOWCASETIME);
            ShowcaseCanvas.SetActive(false);
            ShowcaseCam.gameObject.SetActive(false);
            Destroy(temp);
        }
    }

    private void CosmeticsShowcase()
    {
        unlockSound.Play();
        showcaseParticles.Play();
    }

    public void UnlockAll() //Console Command
    {
        foreach (var item in Abilities)
        {
            item.EnableAbility(true);
        }
    }
}
