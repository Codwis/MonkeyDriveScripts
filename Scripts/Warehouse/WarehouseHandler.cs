using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WarehouseHandler : MonoBehaviour
{
    #region Singleton
    public static WarehouseHandler instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public ShelfScript[] Shelves;
    public Codwis.Event EventToCall;

    public MovementController Player;

    public void CheckForCorrect()
    {
        bool correct = true;
        foreach(var shelf in Shelves)
        {
            if (!shelf.Correct) correct = false;
        }

#if UNITY_EDITOR
        if (EventToCall != null) EventToCall.Invoke(Player.transform);
#else
        if (correct)
            {
                if (EventToCall != null) EventToCall.Invoke(Player.transform);
                else Debug.Log("Remember Event for Warehouse Handler");

                foreach (var shelf in Shelves) Destroy(shelf);
            }
#endif
    }

}
