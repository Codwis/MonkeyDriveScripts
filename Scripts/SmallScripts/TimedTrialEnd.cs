using UnityEngine;

public class TimedTrialEnd : MonoBehaviour
{
    //Small script for timer end collider
    private TimedTrialEvent _trialEvent;
    public void Setup(TimedTrialEvent input) //Sets the start trial
    {
        _trialEvent = input;
    }

    private void OnTriggerEnter(Collider other) //Player reaches this calls the main trial
    {
        if(other.transform.root.GetComponentInChildren<CarController_CW>().transform == _trialEvent.TrialTaker && _trialEvent.TrialActive)
        {
            _trialEvent.StopTrial();
        }
    }
}
