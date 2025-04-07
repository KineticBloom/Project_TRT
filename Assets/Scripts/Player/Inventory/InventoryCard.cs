using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

[Serializable]
public class InventoryCard
{
    [SerializeField, ReadOnly]
    public InventoryCardData Data;

    [ReadOnly] public bool HaveOwned = false;
    [ReadOnly] public bool CurrentlyOwn = false;
    [ReadOnly] public int CurrentValue = 0;

    #region ---------- InventoryCardData Accessors ----------

    public string CardName
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard CardName that has not been set"); }
            return Data.CardName;
        }
    }

    public string ID
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard ID that has not been set"); }
            return Data.ID;
        }
    }

    public string Description
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard Description that has not been set"); }
            return Data.Description;
        }
    }

    public Sprite Sprite
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard Sprite that has not been set"); }
            return Data.Sprite;
        }
    }

    public int BaseValue
    {
        get
        {
            if (Data == null) { Debug.LogError("Card has not been set"); throw new System.Exception("Accessing InventoryCard BaseValue that has not been set"); }
            return Data.BaseValue;
        }
    }

    #endregion

    #region ========= [ PUBLIC METHODS ] =========

    public InventoryCard()
    {
    }

    public InventoryCard(InventoryCardData data)
    {
        Data = data;
        CurrentValue = BaseValue;
    }

    /// <summary>
    /// Sets the CurrentValue of the InventoryCard to its BaseValue
    /// </summary>
    public void ResetCurrentValue()
    {
        CurrentValue = BaseValue;
    }

    #endregion

    #region ========= [ PRIVATE METHODS ] =========

    #endregion
}
