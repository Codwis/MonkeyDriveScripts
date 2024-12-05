using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public static InputHandler instance;
    public GameObject console;
    private void Awake()
    {
        instance = this;
#if UNITY_EDITOR

        foreach (SingleInput input in inputs.inputs)
        {
            PlayerPrefs.SetInt(input.inputName, (int)input.keyItself);
        }
#else
        foreach (SingleInput input in inputs.inputs)
        {
            if(!PlayerPrefs.HasKey(input.inputName))
            {
                PlayerPrefs.SetInt(input.inputName, (int)input.keyItself);
            }
        }
#endif
        PlayerPrefs.Save();
    }

    private void Start()
    {
        if(console != null)
        {
            if(IngameDebugConsole.DebugLogManager.Instance == null)
            {
                GameObject.Instantiate(console);
            }
        }
        Freecam.Initialize();
    }

    [Tooltip("This is the inputs scriptableObject which is only needed one expand more inputs if needed")]
    public Inputs inputs;

    public static void ChangeInput(SingleInput newInput) //This just changes a input
    {
        PlayerPrefs.SetInt(newInput.inputName, (int)newInput.keyItself);
        PlayerPrefs.Save();
    }

    public static KeyCode GetInput(string inputToGet) //This will try to get a input otherwise show error using ternary operator
    {
        return PlayerPrefs.HasKey(inputToGet) ? (KeyCode)PlayerPrefs.GetInt(inputToGet) : ShowError();
        
        KeyCode ShowError()
        {
            Debug.LogError("No Key Found " + inputToGet);
            return KeyCode.None;
        }
    }
    public static Sprite GetInputSprite(string input)
    {
        foreach(var t in instance.inputs.inputs)
        {
            if (t.inputName == input) return t.keyImage;
        }

        Debug.Log("No input image found for " + input);
        return null;
    }

}
