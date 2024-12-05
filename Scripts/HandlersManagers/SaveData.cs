using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData //Main data class which will have all the data
{
    public LocationData PlayerLocation;
    public CollectibleData Collectables;
    public UnlockableData Unlockables;
    public ColorData Colors;

    public SaveData(Vector3 position, float rotY, List<string> colIds, List<string> cosmeticIds, List<string> abilityIds, List<CustomizationUiPieceSetup> ui)
    {
        PlayerLocation = new LocationData(position, rotY);
        Collectables = new CollectibleData(colIds);
        Unlockables = new UnlockableData(cosmeticIds, abilityIds);
        Colors = new ColorData(ui);
    }
}

[System.Serializable]
public class LocationData //Data class for player location
{
    public float[] PlayerLocation;
    public float RotY;
    public LocationData(Vector3 spot, float rotY)
    {
        PlayerLocation = new float[3];

        PlayerLocation[0] = spot.x;
        PlayerLocation[1] = spot.y;
        PlayerLocation[2] = spot.z;

        RotY = rotY;
    }
}

[System.Serializable]
public class CollectibleData //Data class for collectibles
{
    public string[] CollectedIds;
    public CollectibleData(List<string> ids)
    {
        CollectedIds = new string[ids.Count];
        for(int i = 0; i < ids.Count; i++)
        {
            CollectedIds[i] = ids[i];
        }
    }
}

[System.Serializable]
public class UnlockableData //Data class for unlockables
{
    public string[] UnlockedIds;
    public string[] AbilityIds;

    public UnlockableData(List<string> cosmeticIds, List<string> abilityIds)
    {
        int i;
        UnlockedIds = new string[cosmeticIds.Count];
        for(i = 0; i < cosmeticIds.Count; i++)
        {
            UnlockedIds[i] = cosmeticIds[i];
        }

        AbilityIds = new string[abilityIds.Count];
        for(i = 0; i < abilityIds.Count; i++)
        {
            AbilityIds[i] = abilityIds[i];
        }
    }
}

[System.Serializable]
public class ColorData
{
    public ColorData(List<CustomizationUiPieceSetup> ui)
    {
        savedColors = new ColorFormat[ui.Count];
        for(int i = 0; i < ui.Count; i++)
        {
            ColorFormat format = new ColorFormat(ui[i].MainMaterial.color);
            savedColors[i] = format;
        }
    }
    public ColorFormat[] savedColors;
}
