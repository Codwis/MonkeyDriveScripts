using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static int CurrentSave = 1;

    private void Start()
    {
        int i = 1;
        foreach(var s in GetComponentsInChildren<Save>(true))
        {
            s.SaveNum = i;
            s.SetColor();
            i++;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CancelButtonCall();
    }

#if UNITY_EDITOR
    public static bool NewGame = true;
#else
    public static bool NewGame = false;
#endif

    public GameObject SavesObject;
    public GameObject CancelButton;
    public GameObject MainButtonsObject;
    public GameObject Settings;
    public void ShowSaves(bool newGame)
    {
        NewGame = newGame;
        EnableButtons(false);

        SavesObject.SetActive(true);
        CancelButton.SetActive(true);
    }
    public void CancelButtonCall()
    {
        SavesObject.SetActive(false);
        CancelButton.SetActive(false);
        Settings.SetActive(false);
        EnableButtons(true);
    }

    public void EnableButtons(bool on)
    {
        MainButtonsObject.SetActive(on);
    }

    public void ShowSettings()
    {
        EnableButtons(false);

        Settings.SetActive(true);
        CancelButton.SetActive(true);
    }
    public void Exit()
    {
        Application.Quit();
    }
}
