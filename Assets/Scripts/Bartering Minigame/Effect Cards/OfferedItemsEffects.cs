using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
#if UNITY_EDITOR
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed
#endif

[System.Serializable]
public class OfferedItemsEffects : EffectCard
{
    #region ======== [ PARAMETERS ] ========

    [InfoBox("This EffectCard will search for all player offered items that satisfy the Item Conditions and modify them according to Item Actions. " +
        "\n\nRuns after the player offers items.")]

    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemCondition> ItemConditions;

    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemAction> ItemActions;


    private List<InventoryCardData> _matchingItems = new List<InventoryCardData>();

    #endregion

    #region ======== [ FUNCTIONS ] ========

    /// <summary>
    /// Adds any items matching the conditions to _matchingItems
    /// </summary>
    /// <returns></returns>
    public override bool DoesActivate(OfferedItems offeredItems, ActivationTime activationTime)
    {
        if (this.activationTime != activationTime && activationTime != ActivationTime.Both) return false;

        _matchingItems.Clear();

        foreach (InventoryCardData item in offeredItems.Items)
        {
            bool addItem = false;

            foreach (IItemCondition condition in ItemConditions)
            {
                if (condition.IsSatisfied(item))
                {
                    addItem = true;
                }
            }

            if (addItem)
            {
                _matchingItems.Add(item);
            }
        }

        return _matchingItems.Count > 0;
    }


    /// <summary>
    /// Apply the ItemActions to items in _matchingItems
    /// </summary>
    public override void Activate(OfferedItems offeredItems)
    {
        foreach (InventoryCardData item in _matchingItems)
        {
            foreach (IItemAction action in ItemActions)
            {
                action.Activate(item);
            }
        }

        Reveal();
    }


    /// <summary>
    /// Sets activationTime to be AfterOffer
    /// </summary>
    public OfferedItemsEffects()
    {
        activationTime = ActivationTime.AfterOffer;
    }

    #endregion
}


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
