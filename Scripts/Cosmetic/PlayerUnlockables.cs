using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnlockables : MonoBehaviour
{
    public List<string> UnlockableIds { get; private set; } = new List<string>();
    public List<string> AbilityIds { get; private set; } = new List<string>();

    public void AddUnlockable(Unlockable unlock, bool loaded = false) //Add new unlockable and activate it
    {
        if (UnlockableIds.Contains(unlock.UnlockableId)) return;

        UnlockableIds.Add(unlock.UnlockableId);
        unlock.Unlock(loaded);
    }
    public void AddAbility(Ability ability, bool loaded = false) //add a new ability and enable it
    {
        if (AbilityIds.Contains(ability.AbilityId)) return;

        AbilityIds.Add(ability.AbilityId);
        ability.EnableAbility(loaded);
    }

    public void LoadData(UnlockableData data) //Loads up the data
    {
        //Goes through all unlocked ids and adds them if they exist
        if(data.UnlockedIds != null)
        {
            foreach (string t in data.UnlockedIds)
            {
                var temp = UnlockablesHandler.instance.GetUnlockable(t);
                if (temp != null) AddUnlockable(temp, true);
            }
        }

        //Same but with abilities
        if (data.AbilityIds != null)
        {
            foreach (string t in data.AbilityIds)
            {
                var temp = UnlockablesHandler.instance.GetAbility(t);
                if (temp != null) AddAbility(temp, true);
            }
        }
    }
}
