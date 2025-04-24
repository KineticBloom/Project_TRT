using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region ======== [ IItemCondition ] ========

public interface IItemCondition
{
    public bool IsSatisfied(InventoryCardData item);
}


[System.Serializable]
public class SearchForTags : IItemCondition
{
    [Tooltip("List of Tags that the action will search for and affect")]
    public List<string> Tags;

    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(InventoryCardData item)
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


[System.Serializable]
public class SearchForItems : IItemCondition
{
    [Tooltip("List of Items that the action will search for and affect")]
    public List<InventoryCardData> Items;


    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    public bool IsSatisfied(InventoryCardData item)
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
