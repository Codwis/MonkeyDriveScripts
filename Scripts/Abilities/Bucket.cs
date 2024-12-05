using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : Ability
{
    public CustomizationScript customizationScript;
    public override void Use()
    {
        base.Use(); //plays audio and effects

        //Shows the customization ui
        customizationScript.HideColorPicker();
        customizationScript.gameObject.SetActive(!customizationScript.gameObject.activeSelf);

        //Show or hide the cursor
        if(customizationScript.gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void HideUi() //Hides this ui
    {
        customizationScript.gameObject.SetActive(false);
        customizationScript.HideColorPicker();
    }
}
