using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SideDreamPauseMenu : MonoBehaviour
{
    public int MainScene;
    public GameObject Settings;
    private void Start()
    {
        ShowMenu(false);
    }

    public void ShowMenu(bool on) //Shows and hides the menu
    {
        Cursor.lockState = on ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = on;
        CancelButton();
        gameObject.SetActive(on);
    }

    public void ShowSettings() //This shows and hides settings
    {
        Settings.SetActive(!Settings.activeSelf);
    }

    public void CancelButton() //Cancel button call
    {
        Settings.SetActive(false);
        ShowConfirm(false);
    }

    [Header("Exit")]
    public GameObject ConfirmExit;
    public string ExitText = "Are you Certain? \n if you are not done it will not save progress in here";
    public TMP_Text exitText;
    public void ShowConfirm(bool on) //Shows the confirm Button and display the warning
    {
        if(on)
        {
            TextWriter.instance.WriteText(ExitText,false, exitText);
        }
        
        ConfirmExit.SetActive(on);
    }

    public void ExitDream() //exits the side dream
    {
        if(MainScene < 0)
        {
            Application.Quit();
            return;
        }
        MainMenu.NewGame = false;
        SceneManager.LoadScene(MainScene + 1);
    }
}
