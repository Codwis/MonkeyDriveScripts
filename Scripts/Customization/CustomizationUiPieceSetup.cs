using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationUiPieceSetup : MonoBehaviour
{
    public TMPro.TMP_Text _text;
    public Image img;

    public Material MainMaterial;
    private CustomizationScript _customization;
    public void Setup(Material mat, string text, int num, CustomizationScript script)
    {
        MainMaterial = mat;
        if (SavingHandler.CurrentSaveData != null)
        {
            var t = SavingHandler.CurrentSaveData.Colors.savedColors;

            Color color = new Color(t[num - 1].R, t[num - 1].G, t[num - 1].B);
            MainMaterial.color = color;
        }
        
        img.color = MainMaterial.color;

        _text.text = text + num.ToString();
        _customization = script;
    }

    public void OnButtonPress()
    {
        _customization.ShowColorPicker(ref MainMaterial, ref img);
    }
}


