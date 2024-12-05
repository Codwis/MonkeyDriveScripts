using UnityEngine;
using TMPro;

public class PlayerTimer : MonoBehaviour
{
    //Small script for getting timer in player
    public TMP_Text TimerText { get; private set; }

    private void Start()
    {
        TimerText = GetComponentInChildren<TMP_Text>();
    }
}
