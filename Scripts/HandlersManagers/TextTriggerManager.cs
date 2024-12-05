using System;
using System.Collections.Generic;
using UnityEngine;

public class TextTriggerManager : MonoBehaviour
{
    [NonSerialized] public static TextTriggerManager instance;
    private void Awake()
    {
        instance = this;
    }

    public List<TextTrigger> triggersToSave = new List<TextTrigger>();

    public void Save()
    {
        foreach (var item in triggersToSave)
        {
            item.Save();
        }
    }
}
