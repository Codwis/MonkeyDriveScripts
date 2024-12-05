using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectables : MonoBehaviour
{
    private Dictionary<System.Type, int> _collectables = new Dictionary<System.Type, int>();
    public List<string> CollectedIds { get; private set; } = new List<string>();
    public PauseMenu PauseMenu;

    public const string BANANA_COUNT = "Banana";
    public const string ODD_COUNT = "Odd";

    private void Start()
    {
        PauseMenu = GameHandler.instance.PauseMenu;
    }
    public void AddCollectable(Collectable collectable, int amount, bool loaded = false)
    {
        if (!GameHandler.Player.Allow && !loaded) return;

        var type = collectable.GetType();

        if (_collectables.ContainsKey(type))
        {
            _collectables[type] += amount;
        }
        else
        {
            _collectables.Add(type, amount);
        }

        if(type == typeof(NormalBanana))
        {
            NormalBanana b = new NormalBanana();
            PlayerPrefs.SetInt(BANANA_COUNT + MainMenu.CurrentSave, GetAmount(b));
        }
        else if(type == typeof(OddBanana))
        {
            OddBanana od = new OddBanana();
            PlayerPrefs.SetInt(ODD_COUNT + MainMenu.CurrentSave, GetAmount(od));
        }

        PlayerPrefs.Save();
        CollectedIds.Add(collectable.CollectableId);
        PauseMenu.UpdateNumbers();
    }

    public int GetAmount(Collectable collectable) //Gets how much of the given collectable there is
    {
        var type = collectable.GetType();
        return _collectables.ContainsKey(type) ? _collectables[type] : 0;
    }


    public void LoadData(CollectibleData data) //Loads up the saved collected ids
    {
        foreach(string str in data.CollectedIds)
        {
            if (CollectedIds.Contains(str)) continue;
            CollectedIds.Add(str);
        }
        var t = CollectableHandler.instance.RemoveCollected(CollectedIds,this);

        foreach(Collectable p in t)
        {
            AddCollectable(p, 1, true);
        }
        for (int i = 0; i < t.Count; i++) Destroy(t[i].gameObject);
    }
}
