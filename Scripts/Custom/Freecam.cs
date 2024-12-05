using IngameDebugConsole;
using UnityEngine;
using System.IO;

public class Freecam
{
    private readonly GameObject freeCamObject;

    private readonly Camera _mainCamera;

    private static Freecam instance;

    private Freecam()
    {
        bool allowFreecam = true;

        if (allowFreecam) //If freecam is allowed then set it up
        {
            //Create a new gameobject and give it controller and a camera
            freeCamObject = new GameObject();
            freeCamObject.AddComponent<FreecamController>();

            var freecam = freeCamObject.AddComponent<Camera>();
            _mainCamera = Camera.main;
            if (_mainCamera == null) return;
            //Then copy settings from main camera to the new camera
            CopySettings(_mainCamera, freecam);
            freeCamObject.SetActive(false); //disable it

            //Add it to the console as a command
            DebugLogConsole.AddCommandStatic("FreeCam", "Allows to fly the camera freely", "FreeCam", typeof(Freecam));
        }
    }

    public static void Initialize() //Gets called from InputHandler just sets up script
    {
        instance = new Freecam();
    }
    public static void FreeCam() 
    {
        if(!instance.freeCamObject.activeSelf) //If its gonna get activated move it to the main camera spot
        {
            instance.freeCamObject.transform.SetPositionAndRotation(instance._mainCamera.transform.position, instance._mainCamera.transform.rotation);
        }
        
        
        //Enable disable the camera depending on the state of it
        instance.freeCamObject.SetActive(!instance.freeCamObject.activeSelf);

        if (GameHandler.Player != null)
        {
            GameHandler.Player.Allow = !instance.freeCamObject.activeSelf;
        }
    }

    private void CopySettings(Camera source, Camera target) //Just copies the settings from source to the target
    {
        target.fieldOfView = source.fieldOfView;
        target.nearClipPlane = source.nearClipPlane;
        target.farClipPlane = source.farClipPlane;
        target.orthographic = source.orthographic;
        target.orthographicSize = source.orthographicSize;
        target.clearFlags = source.clearFlags;
        target.backgroundColor = source.backgroundColor;
        target.cullingMask = source.cullingMask;
        target.depth = source.depth;
    }
}
