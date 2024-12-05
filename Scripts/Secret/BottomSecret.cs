using System.Collections;
using UnityEngine;

public class BottomSecret : MonoBehaviour
{
    [Tooltip("Where player will be put")]public Transform spotToLand;
    
    [Tooltip("Switch skybox to this on trigger")]public Material skyboxNight;
    [Tooltip("Say this on trigger")]public string textToWrite;
    [Tooltip("Changes the fog color to this")]public Color newFogColor;

    private Material _skyboxDefault;
    private Color _defaultFogCol;
    private void Start()
    {
        _skyboxDefault = RenderSettings.skybox;
        _defaultFogCol = RenderSettings.fogColor;

        IngameDebugConsole.DebugLogConsole.AddCommand("SwitchSky", "Switches between 2 Skies", SwitchSky);
    }
    private bool activated = false;
    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        activated = true;
        TextWriter.instance.WriteText(textToWrite, fade: false);
        SwitchSky();

        GameHandler.Player.Rb.velocity = Vector3.zero;
        GameHandler.Player.transform.SetPositionAndRotation(spotToLand.position, spotToLand.rotation);

        StartCoroutine(smallDelay());
        IEnumerator smallDelay()
        {
            yield return new WaitForSeconds(2);
            activated = false;
        }
    }

    public void SwitchSky()
    {
        bool day = RenderSettings.skybox == _skyboxDefault;

        RenderSettings.skybox = day ? skyboxNight : _skyboxDefault;
        RenderSettings.fogColor = day ? newFogColor : _defaultFogCol;
        RenderSettings.sun.color = day ? newFogColor : _defaultFogCol;
    }

}
