using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSpinner : MonoBehaviour
{
    private Material Skybox;
    [Tooltip("How fast does it spin")]public float Speed;

    private float _current = 0;
    private void Start()
    {
        Skybox = RenderSettings.skybox;
        _current = Skybox.GetFloat("_Rotation");
    }
    private void FixedUpdate() //Spins the skybox around and resets when full loop
    {
        _current += Time.deltaTime * Speed;
        if (_current >= 360) _current = 0;
        Skybox.SetFloat("_Rotation", _current);
        
    }

    public IEnumerator SpeedUp(AsyncOperation operation) //This Speeds up the skybox 
    {
        while(Speed < 1000 && !operation.isDone)
        {
            Speed += Speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        operation.allowSceneActivation = true;
    }
}
