using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// YOUTUBE VIDEO REFERENCED: https://www.youtube.com/watch?v=1mf730eb5Wo&ab_channel=SasquatchBStudios

public class SaveSystem
{
    private static SaveData _saveData = new();

    [System.Serializable]
    public struct SaveData
    {
        public NPCSaveData npcSaveData;
    }

    #region ========== [ PUBLIC METHODS ] ===========

    // SAVE

    /// <summary>
    /// Saves the game. Specifically, the NPC Effect Cards' "revealed" status
    /// </summary>
    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }


    /// <summary>
    /// Loads the game. Specifically, the NPC Effect Cards' "revealed" status
    /// </summary>
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }


    /// <summary>
    /// Checks if there is save data available.
    /// </summary>
    /// <returns>True if save data exists, otherwise false.</returns>
    public static bool HasSaveData()
    {
        string saveFile = SaveFileName();

        if (!File.Exists(saveFile))
        {
            return false;
        }

        try
        {
            string saveContent = File.ReadAllText(saveFile);
            JsonUtility.FromJson<SaveData>(saveContent);
            return true;
        }
        catch
        {
            // If the file exists but contains invalid data, return false
            return false;
        }
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========

    private static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    private static void HandleSaveData()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot save data, GameManager is null");
            return;
        }

        GameManager.Instance.Save(_saveData.npcSaveData);

        // Set up the serializable data
        _saveData.npcSaveData.serializableData = _saveData.npcSaveData.Serializable();
    }

    private static void HandleLoadData()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot load data, GameManager is null");
            return;
        }

        if (GameManager.Inventory == null)
        {
            Debug.LogError("Cannot load Inventory, GameManager.Inventory is null");
            return;
        } else
        {
            // GameManager.Inventory.Load(_saveData.inventoryData);
        }
    }

    #endregion
}

[System.Serializable]
public class NPCSaveData
{
    public Dictionary<string, List<bool>> effectCardData;
    public List<Pair<string, List<bool>>> serializableData;
    public List<Pair<string, List<bool>>> Serializable()
    {
        return Serialize.FromDict(effectCardData);
    }

    public void FromSerialized(List<Pair<string, List<bool>>> serializedData)
    {
        effectCardData = Serialize.ToDict(serializedData);
    }
}
