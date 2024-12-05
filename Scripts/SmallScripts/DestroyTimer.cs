using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [Tooltip("How many seconds should the thing last")] public float secondsTillDestroy;
    [Tooltip("Where should the thing move towards Empty for nowhere")] public Vector3 movDirection = Vector3.zero;
    [Tooltip("Decrease the scale smoothly to 0 ?")] public bool makeSmall = true;
    [Tooltip("Light which should fade will try get object")] public Light lightToFade;
    public bool instantStart = true;

    private void Start()
    {
        if (instantStart)
        {
            transform.parent = null;
            lightToFade = GetComponent<Light>();
            StartCoroutine(Destroy());
        }
    }

    private void Update()
    {
        //Moves the Object
        if (movDirection != Vector3.zero)
        {
            transform.position += movDirection * Time.deltaTime;
        }
    }

    //Timer Setup
    public IEnumerator Destroy()
    {
        //Makes the object smaller and less bright with time based system

        float startTime = Time.time;
        float lightRange = 0;
        if(lightToFade != null) lightRange = lightToFade.range;


        Vector3 scale = transform.localScale;

        while (true)
        {
            float t;
            //Make the light fade out
            if (lightToFade != null)
            {
                t = (Time.time - startTime) / secondsTillDestroy;
                lightToFade.range = Mathf.Lerp(lightRange, 0, t);
            }

            //Make object smaller
            if (makeSmall)
            {
                t = (Time.time - startTime) / secondsTillDestroy;
                transform.localScale = Vector3.Lerp(scale, Vector3.zero, t);
            }

            //Destroy object after time passed
            if (startTime + secondsTillDestroy < Time.time)
            {
                Destroy(gameObject);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
