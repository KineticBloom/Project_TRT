using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#region ======== [ IItemCondition ] ========

public interface IItemCondition
{
    public bool IsSatisfied(TradeInfo tradeInfo);
}

/// <summary>
/// Offer # items with tag and Offer Item(s) with tag A and tag B
/// </summary>
[System.Serializable]
public class CountItemsWithTags : IItemCondition
{
    [Header("Conditions")]
    [Tooltip("Tag that the action will search for and affect")]
    public List<string> Tags;
    [Tooltip("How many items with the tag are required to satisfy this condition?")]
    [Range(1.0f, 4.0f)]
    public int MinAmount = 1;
    [Tooltip("Do the amount of items offered have to be exactly the same as Min Amount?")]
    public bool ExactAmount;

    /// <summary>
    /// Do enough items have these tags?
    /// </summary>
    public bool IsSatisfied(TradeInfo tradeInfo)
    {
        List<InventoryCardData> itemsWithTags = new List<InventoryCardData>();

        // fill itemsWithTag with items that have Tag
        foreach (InventoryCardData possibleItem in tradeInfo.OfferedItems.Items)
        {
            var offeredTagsLower = possibleItem.Tags.Select(tag => tag.ToLower());
            var requiredTagsLower = Tags.Select(tag => tag.ToLower());

            if (requiredTagsLower.All(tagLower => offeredTagsLower.Contains(tagLower)))
            {
                itemsWithTags.Add(possibleItem);
            }
        }

        // If we have less than the amount needed, return false
        if (itemsWithTags.Count < MinAmount)
        {
            return false;
        }

        // If we need the exact amount and we don't have it, return false
        if (ExactAmount && itemsWithTags.Count != MinAmount)
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// Offer an Item with tag A but not tag B
/// </summary>
[System.Serializable]
public class TagsExclusive : IItemCondition
{
    [Header("Conditions")]
    [Tooltip("Tag that the action will search for and affect")]
    public List<string> RequiredTags;
    [Tooltip("Tag that the action will exclude")]
    public List<string> ExcludedTags;
    [Tooltip("How many items with these conditions are required to satisfy this condition?")]
    [Range(1.0f, 4.0f)]
    public int MinAmount = 1;
    [Tooltip("Do the amount of items offered have to be exactly the same as Min Amount?")]
    public bool ExactAmount = false;

    /// <summary>
    /// Compares 2 Tags and checks if you have items that match it
    /// </summary>
    public bool IsSatisfied(TradeInfo tradeInfo)
    {
        List<InventoryCardData> itemsWithTags = new List<InventoryCardData>();

        var requiredTagsLower = RequiredTags.Select(tag => tag.ToLower());
        var excludedTagsLower = ExcludedTags.Select(tag => tag.ToLower());

        // fill itemsWithTag with items that have Tag
        foreach (InventoryCardData possibleItem in tradeInfo.OfferedItems.Items)
        {
            var offeredTagsLower = possibleItem.Tags.Select(tag => tag.ToLower());

            // Skip if it doesn't have required tags
            if (!requiredTagsLower.All(tag => offeredTagsLower.Contains(tag))) continue;

            // Skip if it has the Excluded tags
            if (excludedTagsLower.Any(tag => offeredTagsLower.Contains(tag))) continue;
            
            itemsWithTags.Add(possibleItem);
            
        }

        // If we have less than the amount needed, return false
        if (itemsWithTags.Count < MinAmount)
        {
            return false;
        }

        // If we need the exact amount and we don't have it, return false
        if (ExactAmount && itemsWithTags.Count != MinAmount)
        {
            return false;
        }

        return true;
    }
}

/// <summary>
/// Offer Specific Item and Offer # Specific items
/// eg. [a] or [a, b] (any) or [a, b] (all) or [a, a]
/// </summary>
[System.Serializable]
public class CountSpecificItems : IItemCondition
{
    [Tooltip("List of Items that the action will search for and affect")]
    public List<InventoryCardData> Items;
    [Tooltip("Are all of the items in the Items list necessary for the Effect Card?")]
    public bool NeedAllItems;

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(TradeInfo tradeInfo)
    {
        if (Items.Count == 0) return true;

        // [a], [a, b] (any)
        if (!NeedAllItems)
        {
            foreach (InventoryCardData searchedItem in Items)
            {
                foreach (InventoryCardData offeredItem in tradeInfo.OfferedItems.Items)
                {
                    if (offeredItem.IsSame(searchedItem)) return true;
                }
            }
            return false;
        }

        // [a, b] (all) and [a, a]
        // NeedAllItems == true
        Dictionary<string, int> RequiredItemCounts = new();
        Dictionary<string, int> OfferedItemCounts = new();

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
        return true;
    }
}

/// <summary>
/// Offer # of items
/// </summary>
[System.Serializable]
public class ItemCount : IItemCondition
{
    [Tooltip("How many items are required to activate this condition?")]
    [Range(1.0f, 4.0f)]
    public int MinAmount = 1;

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(TradeInfo tradeInfo)
    {
        return tradeInfo.OfferedItems.Items.Count >= MinAmount;
    }
}

/// <summary>
/// Checks for Dialogue or World Interaction
/// </summary>
[System.Serializable]
public class CheckForFlag : IItemCondition
{
    [Tooltip("Which FlagID do we check for?")]
    public string FlagID = "";

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(TradeInfo tradeInfo)
    {
        return GameManager.FlagTracker.GetFlag(FlagID);
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