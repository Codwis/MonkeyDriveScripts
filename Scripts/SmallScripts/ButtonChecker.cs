using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChecker : Interactable
{
    public AudioClipPreset failSound;
    public AudioClipPreset correctSound;

    public ThrowTrigger[] triggersNeeded;

    public Codwis.EndEvent eventToCall;

    public override void Interact(Transform source)
    {
        bool accept = true;

        foreach(ThrowTrigger trig in triggersNeeded)
        {
            if (!trig.Correct) 
            {
                accept = false;
                break;
            }
        }

        After(accept, source);
    }

    private void After(bool correct, Transform source)
    {
        GameHandler.instance.PlayEffectAudio(correct ? correctSound : failSound);

        if (correct) eventToCall.Invoke(source);
    }
}
