using System.Collections;
using UnityEngine;

public class ScannerBubble : MonoBehaviour
{
    [Tooltip("How long does the bubble grow then gets destroyed")]public float maxTime = 10f;
    [Tooltip("The rate the bubble grows")]public float growRate = 10f;
    [Tooltip("This object will be cloned at the collectables found position has the ScanFound.cs on it")]public GameObject foundObjectShine;

    private void Start()
    {
        StartCoroutine(Grow());
    }

    private IEnumerator Grow() //Main Method grows the bubble with time
    {
        float startTime = Time.time;

        while(Time.time - startTime < maxTime)
        {
            transform.localScale += Vector3.one * (Time.deltaTime * growRate);
            yield return new WaitForEndOfFrame();
        }

        ///After its done get destroyed
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collectable>())
        {
            if (foundObjectShine != null)
            {
                Instantiate(foundObjectShine, other.transform.position, foundObjectShine.transform.rotation);
            }
            else Debug.LogError("NO OBJECT SHINE in" + gameObject);
        }
    }
}
