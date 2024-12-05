using System.Collections;
using System.Collections.Generic;
using System.IO;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsoleCommandList : MonoBehaviour
{
    public static bool xtraCommands = false;

    private void Start()
    {
#if UNITY_EDITOR
        xtraCommands = true;
#else ///only allow commands to people who adds simple txt file to their files
        string exefolderpath = Path.GetDirectoryName(Application.dataPath);
        xtraCommands = File.Exists(exefolderpath + "XtraCmds.txt");
#endif

        DontDestroyOnLoad(gameObject);
        DebugLogConsole.AddCommand("HideUI", "Makes the player unable to die in certain places", HideUI);
    }

    [ConsoleMethod("Debug.DeleteAllPlayerPrefs", "Don't use this it will reset all")]
    public static void DebugDeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [ConsoleMethod("LoadScene", "Loads up the given scene", "Index Scene Number")]
    public static void LoadScene(int index)
    {
        if(index < SceneManager.sceneCountInBuildSettings && index >= 0)
        {
            Debug.Log("Loading Up da world");
            SceneManager.LoadScene(index);
        }
        else
        {
            Debug.Log("Does not exist");
        }
    }


    [ConsoleMethod("ChangeSpeed", "Changes the speed of the car", "NewSpeed default ~200")]
    public static void ChangeSpeed(float newSpeed)
    {
        if(GameHandler.Player != null)
        {
            GameHandler.Player.TorqueAmount = newSpeed;
            Debug.Log("Speed Updated");
        }
        else
        {
            Debug.Log("Cant Do it Here m8");
        }
    }

    [ConsoleMethod("ChangeBrakeForce", "Changes the braking power of the car", "NewForce default ~50")]
    public static void ChangeBrakeForce(float newForce)
    {
        if (GameHandler.Player != null)
        {
            GameHandler.Player.BrakingTorque = newForce;
            Debug.Log("Braking Power Updated");
        }
        else
        {
            Debug.Log("Cant Do it Here m8");
        }
    }

    [ConsoleMethod("ChangeTurnAngle", "Changes the turning angle of the car", "NewAngle Default ~40")]
    public static void ChangeTurnAngle(float newAngle)
    {
        if (GameHandler.Player != null)
        {
            newAngle = Mathf.Clamp(newAngle, 0, 60);
            GameHandler.Player.TurnAngle = newAngle;
            Debug.Log("Turn Angle Updated");
        }
        else
        {
            Debug.Log("Cant Do it Here m8");
        }
    }

    [ConsoleMethod("ChangeWiggleStrength", "Changes the power of the wiggling in the car", "NewPower Default ~2000")]
    public static void ChangeWiggleStrength(float newPower)
    {
        if (GameHandler.Player != null)
        {
            GameHandler.Player.wiggleStrenght = newPower;
            Debug.Log("Wiggle Strength Updated");
        }
        else
        {
            Debug.Log("Cant Do it Here m8");
        }
    }

    [ConsoleMethod("EnableBrakes", "Enables or Disables Brakes RIGHT MOUSE BUTTON is for braking", "ON Default ~False")]
    public static void EnableBrakes(bool on) //Gets all abilities and unlocks them command
    {
        if(GameHandler.Player != null)
        {
            GameHandler.Player.BrakesOn = on;
            Debug.Log("Brakes were turned " + (on ? "ON" : "OFF") + " RIGHT MOUSE BUTTON TO USE THEM");
        }
        else
        {
            Debug.Log("Can't Do that here");
        }
    }


    [ConsoleMethod("UnlockAll", "Unlocks all the abilities")]
    public static void UnlockAll() //Gets all abilities and unlocks them command
    {
        var handler = UnlockablesHandler.instance;
        if(handler != null)
        {
            handler.UnlockAll();
            Debug.Log("All Abilities Activated");
        }
        else
        {
            Debug.Log("Can't do it here");
        }
    }

    [ConsoleMethod("God", "Makes the player Immortal")]
    public static void GodMode(bool on) //Makes u immune to damage in castle
    {
        var player = GameObject.FindFirstObjectByType<FightMovementController>();

        if(SceneManager.GetActiveScene().name.Equals("FinalStage"))
        {
            TextWriter.instance.WriteText("No, I Don't Think I will let you use that", fade: false);
            return;
        }

        if(player != null)
        {
            Debug.Log("God Mode is now " + (on ? "ON" : "OFF"));
            player._stats.allowDamage = !on;
        }
        else
        {
            Debug.Log("Wrong place ...");
        }
    }

    private List<Canvas> enableThese;
    private bool on = true;
    public void HideUI() //Hides or shows the ui
    {
        if(on)
        {
            enableThese = new List<Canvas>();
            foreach (var item in FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (GetComponent<Canvas>() == item) continue;
                else
                {
                    enableThese.Add(item);
                    item.enabled = false;
                }
            }
            on = false;
        }
        else
        {
            on = true;
            foreach (var item in enableThese)
            {
                item.enabled = true;
            }
        }
    }

}
