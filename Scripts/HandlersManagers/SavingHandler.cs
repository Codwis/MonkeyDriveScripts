using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SavingHandler
{
    public static void Save(int saveNum, Vector3 position,float rotY, List<string> collectedIds, List<string> cosmeticIds, List<string> abilityIds, List<CustomizationUiPieceSetup> ui) //Writes the saved data to the drive
    {
        Debug.Log("SAVING " + saveNum);

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player"+ saveNum +".SavedLot";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(position, rotY, collectedIds, cosmeticIds, abilityIds, ui);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData CurrentSaveData = null;
    public static SaveData Load(int saveNum) //Reads the saved data
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player"+ saveNum +".SavedLot";
        if(File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = (SaveData)formatter.Deserialize(stream);
            CurrentSaveData = data;

            stream.Close();
            return data;
        }
        else
        {
            return null;
        }
    }
}
