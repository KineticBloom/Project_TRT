using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameEnums;

[Serializable]
public class NPC
{
    #region ======== [ VARIABLES ] ========

    [SerializeField, ReadOnly]
    public NPCData Data;

    [NonSerialized] public Dictionary<InventoryCardData, InventoryCardData> journalKnownTrades;

    [ReadOnly] public List<Pair<InventoryCardData, InventoryCardData>> SerializedKnownTrades;

    #endregion

    #region ======== [ NPCData Accessors ] ========

    public string Name
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Name that has not been set"); }
            return Data.Name;
        }
    }

    public Sprite Icon
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Icon that has not been set"); }
            return Data.Icon;
        }
    }

    public string Bio
    {
        get
        {
            if (Data == null) { Debug.LogError("NPC has not been set"); throw new System.Exception("Accessing NPC Bio that has not been set"); }
            return Data.Bio;
        }
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    public NPC(NPCData data)
    {
        Data = data;
        
        journalKnownTrades = new Dictionary<InventoryCardData, InventoryCardData>();
        UpdatedKnownTrades();
    }

    /// <summary>
    /// Adds known trade to the journal
    /// </summary>
    /// <param name="playerCard">Inventory Card that the player provides the NPC</param>
    /// <param name="oppCard">Inventory Card that the NPC trades for the playerCard</param>
    public void AddJournalKnownTrade(InventoryCardData playerCard, InventoryCardData oppCard)
    {
        journalKnownTrades.TryAdd(playerCard, oppCard);

        UpdatedKnownTrades();
    }

    /// <param name="playerCard">Card the player trades in</param>
    /// <returns>Card the NPC is know to give in exchange for playerCard</returns>
    public InventoryCardData GetKnownTrade(InventoryCardData playerCard)
    {
        if (playerCard == null || !journalKnownTrades.ContainsKey(playerCard))
            return null;

        return journalKnownTrades[playerCard];
    }

    public void LoadFromSerialized()
    {
        journalKnownTrades = Serialize.ToDict(SerializedKnownTrades);
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    private void UpdatedKnownTrades()
    {
        SerializedKnownTrades = Serialize.FromDict(journalKnownTrades);
    }

    #endregion
}