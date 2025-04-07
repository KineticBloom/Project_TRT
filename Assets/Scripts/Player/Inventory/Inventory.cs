using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;

public class Inventory : MonoBehaviour
{
    [Header("Global Card Info")]
    public AllInventoryCardDatas AllCardDatas;

    [Space]
    [Header("Inventory")]
    public List<InventoryCardData> StartingCards;
    [SerializeField, ReadOnly] private List<InventoryCardData> Cards;
    
    public event Action OnInventoryUpdated;

    [HideInInspector] public float inventoryLastUpdateTime;

    // Helper Classes =============================================================================

    // Enums for Sorting
    public enum SortParameters { 
        NAME, 
        ID, 
        TYPE,
        BASEVALUE
    }

    public enum SortOrder { 
        ASCENDING, 
        DESCENDING 
    }

    // Misc Internal Variables ====================================================================

    private NotificationUI _notificationUi;

    // Initializers ===============================================================================

    private void Awake()
    {
        if (AllCardDatas == null)
        {
            Debug.LogError("Inventory: AllCardDatas has not been set.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Cards = new List<InventoryCardData>();

        foreach (InventoryCardData card in StartingCards) {
            AddCard(card);
        }

        // Cache refs.
        if (GameManager.Instance != null && GameManager.MasterCanvas != null) {
            _notificationUi = GameManager.MasterCanvas.GetComponent<InGameUi>().Notification;
        }
        
        GameManager.FlagTracker.UpdateICFlags();
    }

    #region ---------- Public Methods ----------
    public List<InventoryCardData> Get()
    {
        List<InventoryCardData> returnList = new List<InventoryCardData>();
        if (Cards == null) return returnList;

        foreach (InventoryCardData card in Cards) {
            returnList.Add(card);
        }
        return returnList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    /// <param name="withoutNotification">Will the Notification System not inform the player that a card has been added?</param>
    public void AddCard(InventoryCardData card, bool withoutNotification = false)
    {
        if (card == null) return;

        // Find card in AllCards and add it to the current inventory
        InventoryCardData newCard = null;
        foreach (InventoryCardData possibleNewCard in AllCardDatas.datas) {
            if (possibleNewCard == card) {
                newCard = possibleNewCard;
                break;
            }
        }
        if (newCard == null) {
            Debug.LogError($"Could not find {card.ID}, card does not exist in AllCards");
            return;
        }


        Cards.Add(newCard);


        OnInventoryUpdated?.Invoke();
        inventoryLastUpdateTime = Time.time;
        GameManager.FlagTracker.SetFlag(card, true);

        // Finally, send a ping to our notificationUI.

        if (_notificationUi && !withoutNotification) {
            _notificationUi.Notify($"Obtained {card.CardName}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="card"></param>
    /// <param name="withoutNotification">Will the Notification System not inform the player that a card has been removed?</param>
    public void RemoveCard(InventoryCardData card, bool withoutNotification = false)
    {
        if (!HasCard(card)) {
            Debug.LogError("Cannot remove card. Card is not in inventory.");
            return;
        }

        Cards.Remove(card);

        OnInventoryUpdated?.Invoke();
        inventoryLastUpdateTime = Time.time;
        GameManager.FlagTracker.SetFlag(card, false);

        // Finally, send a ping to our notificationUI.

        if (_notificationUi && !withoutNotification) {
            _notificationUi.Notify($"Lost {card.CardName}");
        }
    }

    /// <summary>
    /// Is the InventoryCardData in the Inventory?
    /// </summary>
    /// <returns></returns>
    public bool HasCard(InventoryCardData cardData)
    {
        if (cardData == null) {
            Debug.LogError("Inventory: HasCard: null reference exception");
            return false;
        }

        foreach (InventoryCardData card in Cards)
        {
            if (card.ID == cardData.ID) return true;
        }
        return false;
    }

    public void Clear()
    {
        Cards.Clear();
        OnInventoryUpdated?.Invoke();
        GameManager.FlagTracker.ResetFlags();
    }

    public void Print()
    {
        string printString = "[\n";
        foreach (InventoryCardData card in Cards) {
            printString += $"[{card.CardName}, {card.ID},\"{card.Description}\"\n";
        }

        printString += "]";
        Debug.Log(printString);
    }

    /// <summary>
    /// Returns the card with id, null if one cannot be found
    /// </summary>
    /// <param name="id">The id to search for.</param>
    /// <returns></returns>
    public InventoryCardData GetCardByID(string id)
    {
        foreach (InventoryCardData card in Cards) {
            if (card.ID == id) return card;
        }

        return null;
    }

    /// <summary>
    /// Returns the first card with cardName, null if one cannot be found
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public InventoryCardData GetCardByName(string cardName)
    {
        foreach (InventoryCardData card in Cards) {
            if (card.CardName == cardName) return card;
        }

        return null;
    }

    /// <summary>
    /// Returns a List of all cards with cardName
    /// </summary>
    /// <param name="cardName">The cardName to search for.</param>
    /// <returns></returns>
    public List<InventoryCardData> GetCardsByName(string cardName)
    {
        List<InventoryCardData> returnList = new List<InventoryCardData>();
        foreach (InventoryCardData card in Cards) {
            if (card.CardName == cardName) {
                returnList.Add(card);
            }
        }

        return returnList;
    }

    /// <summary>
    /// Sorts the cards in the inventory by a given parameter and in a given order
    /// </summary>
    /// <returns></returns>
    public void Sort(SortParameters sortParameter, SortOrder sortOrder)
    {
        Comparison<InventoryCardData> comparison = sortParameter switch {
            SortParameters.NAME => (card1, card2) => string.Compare(card1.CardName, card2.CardName, true),
            SortParameters.ID => (card1, card2) => string.Compare(card1.ID, card2.ID, true),
            SortParameters.BASEVALUE => (card1, card2) => string.Compare(card1.BaseValue.ToString(), card1.BaseValue.ToString(), true),
            _ => null
        };

        // If descending order is selected, reverse the comparison
        if (sortOrder == SortOrder.DESCENDING) {
            var originalComparison = comparison;
            comparison = (card1, card2) => -originalComparison(card1, card2);
        }

        Cards.Sort(comparison);
        OnInventoryUpdated?.Invoke();
        inventoryLastUpdateTime = Time.time;
    }

    /// <summary>
    /// Resets the CurrentValues of all InventoryCardDatas to their BaseValue
    /// </summary>
    public void ResetAllCardValues()
    {
        foreach (InventoryCardData card in AllCardDatas.datas)
        {
            card.SetCurrentValue(card.BaseValue);
        }
    }

    #endregion

    #region ---------- Private Methods ----------

    #endregion

    #region ---------- Save and Load ----------

    //public void Save(ref InventorySaveData data, bool clearInventory)
    //{
    //    data.AllCards = AllCards;
    //    data.Cards = Cards;
    //}

    //public void Load(InventorySaveData data)
    //{
    //    AllCards = data.AllCards;
    //    Cards = data.Cards;
    //}

    #endregion
}

//[System.Serializable]
//public struct InventorySaveData
//{
//    public List<InventoryCard> AllCards;
//    public List<InventoryCard> Cards;
//}