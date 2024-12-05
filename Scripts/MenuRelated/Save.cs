using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Save : MonoBehaviour
{
    public int SaveNum { private get; set; }
    public void SetColor() //Sets the save color if there is a save blue otherwise cyan
    {
        Image im = GetComponent<Image>();
        string path = Application.persistentDataPath + "/player" + SaveNum + ".SavedLot";
        if (File.Exists(path))
        {
            im.color = Color.blue;
        }
        else
        {
            im.color = Color.cyan;
        }
    }
    public void SetCurrentSave() //Sets the current save and starts scene transition
    {
        MainMenu.CurrentSave = SaveNum;
        AsyncOperation op = SceneManager.LoadSceneAsync(2);
        op.allowSceneActivation = false;
        StartCoroutine(GetComponentInParent<SkyboxSpinner>().SpeedUp(op));
    }
}
