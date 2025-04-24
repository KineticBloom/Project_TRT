using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

#region ======== [ IItemCondition ] ========

public interface IItemCondition
{
    public bool IsSatisfied(InventoryCardData item, TradeInfo tradeInfo);
}


[System.Serializable]
public class SearchForTags : IItemCondition
{
    [Tooltip("List of Tags that the action will search for and affect")]
    public List<string> Tags;

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(InventoryCardData item, TradeInfo tradeInfo)
    {
        foreach (string tag in Tags)
        {
            foreach (string itemTag in item.Tags)
            {
                if (itemTag.ToLower().Equals(tag.ToLower()))
                {
                    return true;
                }
            }
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
        foreach (string requiredItemID in RequiredItemCounts.Keys)
        {
            if (item.ID == requiredItemID) return true;
        }

        return false;
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
