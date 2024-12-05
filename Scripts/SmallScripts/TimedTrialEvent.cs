using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TimedTrialEvent : MonoBehaviour
{
    [Header("Main")]
    [Tooltip("If time runs out this is where player will be goin")] public Transform ResetSpot;
    [Tooltip("What is the Endspot it needs trigger collider")] public TimedTrialEnd EndSpot;
    [Tooltip("How much time is there in the trial Seconds")] public float TrialSeconds = 3;

    [Header("Cosmetic")]
    [Tooltip("The audio which gets played at start")]public AudioClipPreset StartAudio;
    [Tooltip("This gets looped every cycle of timer")]public AudioClipPreset LoopAudio;
    [Tooltip("This is played if player reaches in time")] public ParticleSettings WinParticles;
    [Tooltip("This is played if player reaches in time")]public AudioClipPreset WinAudio;

    private TMP_Text _timeLeftText;
    private BarrelMover[] _obstacles;
    public Transform TrialTaker { get; private set; }
    public bool TrialActive { get; private set; }

    private void Start()
    {
        if (EndSpot != null) EndSpot.Setup(this);
        else Debug.LogError("No EndSpot assigned to " + gameObject.name);

        _obstacles = GetComponentsInChildren<BarrelMover>();
    }


    private void StartTrial() //Starts the trial
    {
        //Variables set
        TrialActive = true;
        GameHandler.TrialActive = true;

        //Enable the obstacles
        foreach (var item in _obstacles)
        {
            item.Activate(true, TrialTaker.gameObject.layer);
        }

        //Play audio
        if (StartAudio.Clip != null) GameHandler.instance.PlayEffectAudio(StartAudio);

        //Start the timer
        StartCoroutine(TrialTimer());
    }

    private void OnTriggerEnter(Collider other) //Starts the trial
    {
        if(!TrialActive)
        {
            //Set the taker
            TrialTaker = other.transform.root.GetComponentInChildren<CarController_CW>().transform;
            
            //Get the timer and show it
            var temp = TrialTaker.GetComponentInChildren<PlayerTimer>();
            foreach (var rend in temp.GetComponentsInChildren<MeshRenderer>()) rend.enabled = true;
            _timeLeftText = temp.TimerText;

            //Start
            StartTrial(); 
        }
    }

    private const float MIN_TIME = 0.1f;
    private const float MAX_TIME = 3f;
    private const float EXTRA_WAIT = 2;
    private bool InTime = false;
    private IEnumerator TrialTimer() //The main timer courotine
    {
        //Get current time
        float startTime = Time.time;

        //While There is time left 
        while(Time.time - startTime < TrialSeconds)
        {
            //Get the remaining time
            float waitTime = TrialSeconds - (Time.time - startTime);
            //Update the timer text
            _timeLeftText.text = MathF.Round(waitTime,2).ToString();
            //Clamp it so it wont go wild
            waitTime = Mathf.Clamp(waitTime * EXTRA_WAIT / TrialSeconds, MIN_TIME, MAX_TIME);

            yield return new WaitForSeconds(waitTime);
            GameHandler.instance.PlayEffectAudio(LoopAudio);

        }
        if(InTime)
        {
            yield break;
        }

        ResetPlayer();
    }
    private void ResetPlayer() //This resets the player when not in time
    {
        TrialTaker.SetPositionAndRotation(ResetSpot.position, ResetSpot.localRotation);

        //Hide the Timer
        var temp = TrialTaker.GetComponentInChildren<PlayerTimer>();
        foreach (var rend in temp.GetComponentsInChildren<MeshRenderer>()) rend.enabled = false;
        _timeLeftText.text = "";

        //Nullify the taker and disable the trial
        TrialTaker = null;
        TrialActive = false;
        GameHandler.TrialActive = false;

        //Stop the obstacles
        foreach (var item in _obstacles)
        {
            item.Activate(false);
        }
    }

    public void StopTrial() //This stops the trial only when reached in time
    {
        //Variables
        GameHandler.TrialActive = false;
        TrialActive = false;

        InTime = true;
        StopAllCoroutines();
        
        //Play Cosmetics
        if (WinAudio.Clip != null) GameHandler.instance.PlayEffectAudio(WinAudio);
        GameHandler.instance.PlayCustomParticle(WinParticles, TrialTaker.position);

        //Disable the obstacles
        foreach (var item in _obstacles)
        {
            item.Activate(false);
        }
    }
}
