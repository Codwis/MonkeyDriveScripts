using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockingWhispering : MonoBehaviour
{
    [Header("Knocks")]
    public AudioSource knockSource;
    public AudioClip[] knocks;

    [Header("Whispers")]
    public Transform whisperPivot;
    public AudioSource whisperSource;
    public AudioClip[] whispers;
    private bool _whispering = false;

    private const float MIN_DELAY = 5;
    private const float MAX_DELAY = 15;

    private void Start()
    {
        StartCoroutine(Knocking());
    }

    private IEnumerator Knocking()
    {
        yield return new WaitForSeconds(10);
        while(true)
        {
            int rand = Random.Range(0, knocks.Length);
            knockSource.pitch = Random.Range(0.2f, 1.2f);
            knockSource.PlayOneShot(knocks[rand]);
            yield return new WaitForSeconds(Random.Range(MIN_DELAY, MAX_DELAY) + knocks[rand].length);
        }
    }
    private IEnumerator Whisper()
    {
        while (true)
        {
            int rand = Random.Range(0, whispers.Length);
            whisperSource.pitch = Random.Range(0.2f, 1.3f);
            whisperSource.PlayOneShot(whispers[rand]);
            yield return new WaitForSeconds(Random.Range(MIN_DELAY, MAX_DELAY) + whispers[rand].length);
        }
    }

    private void Update()
    {
        if(_whispering)
        {
            whisperPivot.Rotate(Vector3.up * Time.deltaTime * 10 * Random.Range(0, 3));
        }
    }

    public void StartWhispers()
    {
        if (_whispering) return;

        _whispering = true;
        StartCoroutine(Whisper());
    }
}
