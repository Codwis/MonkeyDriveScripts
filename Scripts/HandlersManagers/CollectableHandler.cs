using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectableHandler : MonoBehaviour
{
    public static CollectableHandler instance;
    public Transform ParentToCollectables;
    public List<Collectable> Collectables { get; private set; } = new List<Collectable>();
    public const string BANANA_TOTAL = "BananaTotal";
    public const string ODD_TOTAL = "Odd_total";
    private void Awake()
    {
        instance = this;
        Collectable[] found = ParentToCollectables ? ParentToCollectables.GetComponentsInChildren<Collectable>() : null; //Get all the collectables

        int bananaCount = 0;
        int oddCount = 0;
        if(found != null)
        {
            for (int i = 0; i < found.Length; i++) //and assign them ids
            {
                if (found[i] is NormalBanana)
                {
                    bananaCount++;
                }
                else if (found[i] is OddBanana)
                {
                    oddCount++;
                }
                found[i].CollectableId = i.ToString();
                Collectables.Add((Collectable)found[i]);
            }
        }

        PlayerPrefs.SetInt(BANANA_TOTAL, bananaCount);
        PlayerPrefs.SetInt(ODD_TOTAL, oddCount);
        PlayerPrefs.Save();
    }

    
    public List<Collectable> RemoveCollected(List<string> collectedIds, PlayerCollectables col) //Removes the given collectables with the given ids
    {
        List<Collectable> temp = new List<Collectable>();
        for(int i = 0; i < collectedIds.Count; i++)
        {
            for(int ii = 0; ii < Collectables.Count; ii++)
            {
                if (Collectables[ii].CollectableId == collectedIds[i])
                {
                    temp.Add(Collectables[ii]);
                    break;
                }
            }
        }
        return temp;
    }
}
