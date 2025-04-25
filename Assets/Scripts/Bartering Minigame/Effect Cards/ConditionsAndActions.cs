using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#region ======== [ IItemCondition ] ========

public interface IItemCondition
{
    public bool IsSatisfied(InventoryCardData item, TradeInfo tradeInfo);
}

/// <summary>
/// Offer Item with tag and Offer # Items with tag
/// </summary>
[System.Serializable]
public class ItemsWithTag : IItemCondition
{
    [Header("Conditions")]
    [Tooltip("Tag that the action will search for and affect")]
    public string Tag;
    [Tooltip("How many items with the tag are required to satisfy this condition?")]
    [Range(1.0f, 4.0f)]
    public int Amount = 1;
    [Tooltip("Do the amount of items offered have to be exactly the same as Amount?")]
    public bool ExactAmount;

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(InventoryCardData item, TradeInfo tradeInfo)
    {
        List<InventoryCardData> itemsWithTag = new List<InventoryCardData>();

        // fill itemsWithTag with items that have Tag
        foreach (InventoryCardData possibleItem in tradeInfo.OfferedItems.Items)
        {
            foreach (string itemTag in possibleItem.Tags)
            {
                if (itemTag.ToLower().Equals(Tag.ToLower())) itemsWithTag.Add(possibleItem);
            }
        }

        // If we have less than the amount needed, return false
        if (itemsWithTag.Count < Amount)
        {
            return false;
        }

        // If we need the exact amount and we don't have it, return false
        if (ExactAmount && itemsWithTag.Count != Amount)
        {
            return false;
        }

        // Return true if our item is one of the items with the Tag
        foreach (InventoryCardData possibleItem in itemsWithTag)
        {
            if (possibleItem.IsSame(item)) return true;
        }

        return false;
    }
}

/// <summary>
/// Offer Specific Item and Offer # Specific items
/// eg. [a] or [a, b] (any) or [a, b] (all) or [a, a]
/// </summary>
[System.Serializable]
public class SearchForItems : IItemCondition
{
    [Tooltip("List of Items that the action will search for and affect")]
    public List<InventoryCardData> Items;
    [Tooltip("Are all of the items in the Items list necessary for the Effect Card?")]
    public bool NeedAllItems;

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(InventoryCardData item, TradeInfo tradeInfo)
    {
        if (Items.Count == 0) return true;

        // [a]
        if (Items.Count == 1)
        {
            return Items[0].IsSame(item);
        }

        // [a, b] (any)
        if (!NeedAllItems)
        {
            foreach (InventoryCardData searchedItem in Items)
            {
                // Checking for matching Card Names since the same items can have different ScriptableObjects
                if (searchedItem.IsSame(item))
                {
                    return true;
                }
            }
            return false;
        }

        // [a, b] (all) and [a, a]
        // NeedAllItems == true
        Dictionary<string, int> RequiredItemCounts = new Dictionary<string, int>();
        Dictionary<string, int> OfferedItemCounts = new Dictionary<string, int>();

        // Set up RequiredItemCounts
        foreach (InventoryCardData requiredItem in Items)
        {
            if (!RequiredItemCounts.ContainsKey(requiredItem.ID))
            {
                RequiredItemCounts.Add(requiredItem.ID, 0);
            }
            RequiredItemCounts[requiredItem.ID]++;
        }

        // Set up OfferedItemCounts
        foreach (InventoryCardData offeredItem in tradeInfo.OfferedItems.Items)
        {
            if (!OfferedItemCounts.ContainsKey(offeredItem.ID))
            {
                OfferedItemCounts.Add(offeredItem.ID, 0);
            }
            OfferedItemCounts[offeredItem.ID]++;
        }

        // Reject if there are less OfferedItems
        foreach (string itemID in RequiredItemCounts.Keys)
        {
            if (!OfferedItemCounts.ContainsKey(itemID)) return false;

            if (OfferedItemCounts[itemID] < RequiredItemCounts[itemID]) return false;
        }

        // If the current item is one of the Required Items, return true
        return RequiredItemCounts.ContainsKey(item.ID);
    }
}

#endregion


#region ======== [ IItemActions ] ========
public interface IItemAction
{
    /// <summary>
    /// Applies an effect to an item
    /// </summary>
    /// <param name="item">Item to apply the effect to</param>
    public void Activate(InventoryCardData item);
}


[System.Serializable]
public class MultiplyValue : IItemAction
{
    [Tooltip("The multiplier being applied to items")]
    public float ValueMultiplier = 1f;


    /// <summary>
    /// Multiplies the Current Value of an item
    /// </summary>
    public void Activate(InventoryCardData item)
    {
        item.SetCurrentValue(Mathf.RoundToInt(item.CurrentValue * ValueMultiplier));
    }
}


[System.Serializable]
public class AddValue : IItemAction
{
    [Tooltip("The addend being applied to the items")]
    public int ValueAddend = 0;


    /// <summary>
    /// Adds the Current Value of an item
    /// </summary>
    public void Activate(InventoryCardData item)
    {
        item.SetCurrentValue(item.CurrentValue + ValueAddend);
    }
}


[System.Serializable]
public class SetValue : IItemAction
{
    [Tooltip("The new value of the items")]
    public int Value = 0;


    /// <summary>
    /// Adds the Current Value of an item
    /// </summary>
    public void Activate(InventoryCardData item)
    {
        item.SetCurrentValue(Value);
    }
}

#endregion
