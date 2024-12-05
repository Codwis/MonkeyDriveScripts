using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitInteractable : Interactable
{
    [Tooltip("Which scene to transfer negative num == Quit application")]public int SceneToGo;
    [Tooltip("If scene is negative then this is needed")]public SideDreamPauseMenu PauseMenu;
    [Tooltip("What will be said if changed can be left empty")]public string TextToSayOnExit = "";

    [Header("One time use")]
    public AndroidCutscene cutscene;
    public override void Interact(Transform source)
    {
        base.Interact(source);

        if (SceneToGo < 0)
        {
            PauseMenu.ShowMenu(true);
            PauseMenu.ShowConfirm(true);
        }
        else
        {
            if (cutscene != null) cutscene.Player = source;

            MainMenu.NewGame = false;
            if(TextToSayOnExit != "")
            {
                TextWriter.instance.WriteText(TextToSayOnExit, sceneToGo: SceneToGo, fadeFromStart: true);
            }
            else
            {
                SceneManager.LoadScene(SceneToGo + 1);
            }
        }    
    }
}
