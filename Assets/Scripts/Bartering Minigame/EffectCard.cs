using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed

[System.Serializable]
public class EffectCard
{
    #region ======== [ PUBLIC VARIABLES ] ========

    public enum ActivationTime { BeforeOffer, AfterOffer, Both }

    [Header("Display")]
    [SerializeField, Tooltip("Icon that displays on the effect card")]
    public Texture2D Icon;
    [SerializeField, Tooltip("Description of what the effect card does")]
    public string Description;


    [Header("Effects")]
    [SerializeField, Tooltip("When the Effect Card is activated.")]
    private ActivationTime activationTime = ActivationTime.AfterOffer;

    [SerializeReference, SubclassSelector]
    public List<IAction> Actions = new List<IAction>();

    #endregion

    #region ======== [ PRIVATE VARIABLE ] ========

    // Would Prefer this to tie with the save system
    private bool _revealed = false;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Checks whether the card can activate or not
    /// </summary>
    /// <param name="barteringController">The bartering controller to get info from</param>
    /// <param name="activationTime">When the activation is being attempted</param>
    /// <returns>Whether a boolean of whether </returns>
    public bool DoesActivate(OfferedItems offeredItems, ActivationTime activationTime)
    {
        if (this.activationTime != activationTime && activationTime != ActivationTime.Both) return false;

        foreach (IAction action in Actions)
        {
            if (action == null) continue;

            if (action.CanActivate(offeredItems))
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Activates the effect card
    /// </summary>
    /// <param name="barteringController">The bartering controller to modify info on</param>
    public void Activate(OfferedItems offeredItems)
    {
        foreach (IAction action in Actions)
        {
            if (action == null) continue;

            if (action.CanActivate(offeredItems))
            {
                action.Activate(offeredItems);
            }
        }
    }


    /// <summary>
    /// Reveal the card if not revealed already
    /// </summary>
    /// <param name="activationTime"></param>
    public void Reveal()
    {
        if (_revealed) return;

        // TODO: Animate the Reveal
        _revealed = true;
    }

    #endregion
}


#region ======== [ IActions ] ========

public interface IAction
{
    public bool CanActivate(OfferedItems offeredItems);

    public void Activate(OfferedItems offeredItems);
}


[System.Serializable]
public class SearchForTags : IAction
{
    [Tooltip("List of Tags that the action will search for and affect")]
    public List<string> Tags;

    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemAction> ItemAction;

    private List<InventoryCardData> _matchingItems = new List<InventoryCardData>();


    /// <summary>
    /// Returns whether the an offered items has one of the tags
    /// </summary>
    public bool CanActivate(OfferedItems offeredItems)
    {
        _matchingItems.Clear();

        foreach (InventoryCardData item in offeredItems.Items)
        {
            if (HasAMatchingTag(item.Tags))
            {
                _matchingItems.Add(item);
            }
        }
        return _matchingItems.Count > 0;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="offeredItems">Set of player's offered items to be modified</param>
    public void Activate(OfferedItems offeredItems)
    {
        foreach (IItemAction action in ItemAction)
        {
            foreach (InventoryCardData item in _matchingItems)
            {
                action.Activate(item);
            }
        }
    }


    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    private bool HasAMatchingTag(List<string> itemTags)
    {
        foreach (string tag in Tags)
        {
            foreach (string itemTag in itemTags)
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
public class SearchForItems : IAction
{
    [Tooltip("List of Items that the action will search for and affect")]
    public List<InventoryCardData> Items;

    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemAction> ItemAction;

    private List<InventoryCardData> _matchingItems = new List<InventoryCardData>();


    /// <summary>
    /// Returns whether the an offered items has one of the tags
    /// </summary>
    public bool CanActivate(OfferedItems offeredItems)
    {
        _matchingItems.Clear();

        foreach (InventoryCardData item in offeredItems.Items)
        {
            if (IsMatchingItem(item))
            {
                _matchingItems.Add(item);
            }
        }
        return _matchingItems.Count > 0;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="offeredItems">Set of player's offered items to be modified</param>
    public void Activate(OfferedItems offeredItems)
    {
        foreach (IItemAction action in ItemAction)
        {
            foreach (InventoryCardData item in _matchingItems)
            {
                action.Activate(item);
            }
        }
    }


    /// <summary>
    /// See if any of the tags matches this class' tags
    /// </summary>
    private bool IsMatchingItem(InventoryCardData item)
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

#endregion