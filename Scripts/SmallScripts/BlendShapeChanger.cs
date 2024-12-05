using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class BlendShapeChanger : MonoBehaviour
{
    [Header("Main")]
    [Tooltip("How long should the state stay on before transition again")] public float secTillChange;
    [Tooltip("What rate should the transition happen EXPERIEMENT")] public float chargeRate = 40;
    [Tooltip("Max value on the blend shape normally is 100 and mininum is 0")] public int blendShapeMax = 100, blendShapeMin = 0;

    [Header("Random")]
    [Tooltip("Does the chargeRate need to be randomized?")] public bool randomize = true;
    [Tooltip("Does the size randomize")]public bool randomizeSize = false;
    [Tooltip("It will multiply the charge rate with the random output")] public float minNum = 0.3f, maxNum = 3;

    [Header("Optional")]
    [Tooltip("If there is a light that needs to be changed timely")] public Light lightToChange;
    [Tooltip("If there is a light what is the max intensity")] public float maxIntensity;

    private float currentCharge;
    private SkinnedMeshRenderer smRenderer;
    private AudioSource source;
    private bool on, timeOut = false;
    private void Start()
    {
        source = GetComponent<AudioSource>();
        smRenderer = GetComponent<SkinnedMeshRenderer>();

        float rand;
        if(randomize)
        {
            rand = Random.Range(minNum, maxNum);
            chargeRate *= rand;

            if(source != null)
            {
                source.volume *= rand;
            }
        }
        if(randomizeSize)
        {
            rand = Random.Range(0.5f, 2);
            transform.localScale *= rand;
        }

        //Determine first transition
        if(smRenderer.GetBlendShapeWeight(0) == 0)
        {
            on = true;
        }
        else
        {
            on = false;
        }
    }

    private void Update()
    {
        if (timeOut) return;

        if(on)
        {
            currentCharge += chargeRate * Time.deltaTime;
            smRenderer.SetBlendShapeWeight(0, currentCharge);
            if(lightToChange != null)
            {
                lightToChange.intensity += currentCharge * Time.deltaTime;
                lightToChange.intensity = Mathf.Clamp(lightToChange.intensity, 0, maxIntensity);
            }

            if(currentCharge >= blendShapeMax)
            {
                StartCoroutine(ChangeDir());
            }
        }
        else
        {
            currentCharge -= chargeRate * Time.deltaTime;
            smRenderer.SetBlendShapeWeight(0, currentCharge);

            if (lightToChange != null)
            {
                lightToChange.intensity -= currentCharge * Time.deltaTime;
                lightToChange.intensity = Mathf.Clamp(lightToChange.intensity, 0, maxIntensity);
            }
            if (currentCharge <= blendShapeMin)
            {
                StartCoroutine(ChangeDir());
            }
        }
    }

    private IEnumerator ChangeDir()
    {
        if(source != null)
        {
            source.Stop();
        }

        timeOut = true;
        yield return new WaitForSeconds(secTillChange);

        if(source != null)
        {
            source.Play();
        }

        on = !on;
        timeOut = false;
    }
}
