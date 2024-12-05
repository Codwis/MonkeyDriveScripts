using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerTrigger : MonoBehaviour
{
    public int SceneNum;
    public string TextToSay;
    public bool resetDeaths = false;
    private bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if(other.transform.root.GetComponentInChildren<CarController_CW>() || other.transform.root.GetComponentInChildren<FightMovementController>())
        {
            if (resetDeaths) FinalPhase.DeathCount = 0;
            triggered = true;
            TextWriter.instance.WriteText(TextToSay, true, sceneToGo: SceneNum, fadeFromStart: true);
        }
    }
}
