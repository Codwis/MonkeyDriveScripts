using System.Collections;
using UnityEngine;

public class ScanFound : MonoBehaviour
{
    [Tooltip("How long does the object stay on the found thing")]public float duration;
    private void Start()
    {
        StartCoroutine(TimeDestroy());
    }
    private IEnumerator TimeDestroy() //Real simple destroy timer
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
