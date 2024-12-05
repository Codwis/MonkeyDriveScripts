using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextBubble : MonoBehaviour
{
    public TMP_Text textBox;
    private Transform lookSpot = null;
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Activate(bool on, Transform player)
    {
        gameObject.SetActive(on);
        lookSpot = on ? player : null;
    }

    private void Update()
    {
        if (lookSpot != null) transform.LookAt(lookSpot);
    }
}
