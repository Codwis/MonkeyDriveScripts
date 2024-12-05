using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachTrigger : MonoBehaviour
{
    [Tooltip("The start value of the blend shape")] public float startBlendValue = 100;
    [Tooltip("The end value of the blend shape")] public float endBlendValue = 0;
    [Tooltip("How many seconds does it take from one to another")]public float changeTime = 1f;

    private SkinnedMeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
    }

    private Action callBack;
    public void ToEnd(ref Action call)
    {
        callBack = call;
        StartCoroutine(ValueChanger(startBlendValue, endBlendValue));
    }
    public void ToStart(ref Action call)
    {
        callBack = call;
        StartCoroutine(ValueChanger(endBlendValue, startBlendValue));
    }

    private IEnumerator ValueChanger(float start, float end)
    {
        float currentVal = 0;
        bool negative = start > end;

        while(currentVal <= changeTime)
        {
            currentVal += Time.deltaTime;
            float val = Mathf.Lerp(start, end, currentVal / changeTime);
            _renderer.SetBlendShapeWeight(0, val);

            yield return new WaitForEndOfFrame();
        }
        if(callBack != null)
        {
            callBack.Invoke();
            callBack = null;
        }
    }
}
