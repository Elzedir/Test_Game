using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Save_Manager
{
    public static void SaveGame(Save_Data gameData, int saveSlot)
    {
        string path = Application.persistentDataPath + "/save" + saveSlot + ".json";
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(path, json);
    }

    public static Save_Data LoadGame(int saveSlot)
    {
        string path = Application.persistentDataPath + "/save" + saveSlot + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<Save_Data>(json);
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteSave(int saveSlot)
    {
        string path = Application.persistentDataPath + "/save" + saveSlot + ".json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
    }
}
