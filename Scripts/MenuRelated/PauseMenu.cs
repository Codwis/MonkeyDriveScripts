using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("Main UI")]
    public GameObject PauseMenuRoot;
    public GameObject Settings;
    public GameObject ConfirmExit;

    [Header("Misc")]
    public PlayerCollectables PlayerCollectables;
    public List<CollectableUiItem> UiItems = new List<CollectableUiItem>();

    private void Awake()
    {
        foreach(var i in UiItems)
        {
            i.UiText = i.UiItem.GetComponentInChildren<TMP_Text>();
        }
        
        CloseMenu();
    }

    public void SetPlayerCollectables(PlayerCollectables co)
    {
        PlayerCollectables = co;
        UpdateNumbers();
    }
    public void UpdateNumbers()
    {
        foreach(CollectableUiItem item in UiItems)
        {
            item.UiText.text = ": " + PlayerCollectables.GetAmount(item.Collectable).ToString();
        }
    }

    public void CloseSettings()
    {
        Settings.SetActive(false);
    }
    public void ShowSettings()
    {
        Settings.SetActive(true);
    }

    public void Save()
    {
        GameHandler.Save(MainMenu.CurrentSave, GameHandler.Player.transform, 
            GameHandler.Player.GetComponentInChildren<PlayerCollectables>(), GameHandler.Player.GetComponentInChildren<PlayerUnlockables>());
    }
    public void Load()
    {
        MainMenu.NewGame = false;
        GameHandler.Load();
    }
    
    public void ShowExit()
    {
        ConfirmExit.SetActive(!ConfirmExit.activeSelf);
    }
    public void HideExit()
    {
        ConfirmExit.SetActive(false);
    }
    public void Exit(bool save)
    {
        if(save)
        {
            Save();
        }

        Application.Quit();
    }

    public void Unstuck()
    {
        GameHandler.instance.Unstuck();
    }

    public void CloseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CloseSettings();
        HideExit();
        PauseMenuRoot.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void OpenMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PauseMenuRoot.SetActive(true);
    }

    public bool MenuOpen()
    {
        return PauseMenuRoot.activeSelf;
    }
}

[System.Serializable]
public class CollectableUiItem
{
    public GameObject UiItem;
    [NonSerialized]public TMP_Text UiText;
    [NonSerialized]public int Amount;
    public Collectable Collectable;
}

