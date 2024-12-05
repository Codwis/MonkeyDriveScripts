using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechAi : MonoBehaviour
{
    [Tooltip("The different possible spawn spots")] public Transform[] spawnSpots;
    [Tooltip("The prefab this spins out")] public GameObject spinnerPrefab;
    [Tooltip("The delay between each spinner")] public float maxSecondsTillSpawn;
    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 500 * Time.deltaTime);
    }

    private IEnumerator Spawn()
    {
        while(true)
        {
            int rand = Random.Range(0, spawnSpots.Length);
            Instantiate(spinnerPrefab, spawnSpots[rand].position, spawnSpots[rand].rotation);

            float wait = Random.Range(0, maxSecondsTillSpawn);
            yield return new WaitForSeconds(wait);
        }
    }
}
