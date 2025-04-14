using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed

[System.Serializable]
public abstract class EffectCard
{
    #region ======== [ PUBLIC VARIABLES ] ========

    public enum ActivationTime { BeforeOffer, AfterOffer, Both }

    [Header("Display")]
    [SerializeField, Tooltip("Icon that displays on the effect card")]
    public Sprite Icon;
    [SerializeField, Tooltip("Description of what the effect card does")]
    public string Description;
    [SerializeField, Tooltip("Short Text for Corners")]
    public string Text;

    protected ActivationTime activationTime = ActivationTime.AfterOffer;

    public UnityAction OnRevealed;

    #endregion

    #region ======== [ PRIVATE VARIABLE ] ========

    // Would Prefer this to tie with the save system
    private bool _revealed = false;
    public bool IsRevealed => _revealed;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Checks whether the card can activate or not
    /// </summary>
    /// <param name="barteringController">The bartering controller to get info from</param>
    /// <param name="activationTime">When the activation is being attempted</param>
    /// <returns>Whether a boolean of whether </returns>
    public abstract bool DoesActivate(OfferedItems offeredItems, ActivationTime activationTime);


    /// <summary>
    /// Activates the effect card
    /// </summary>
    /// <param name="barteringController">The bartering controller to modify info on</param>
    public abstract void Activate(OfferedItems offeredItems);


    /// <summary>
    /// Reveal the card if not revealed already
    /// </summary>
    /// <param name="activationTime"></param>
    public void Reveal()
    {
        if (_revealed) return;

        OnRevealed?.Invoke();
        _revealed = true;
    }


    /// <summary>
    /// Reset the Reveal
    /// </summary>
    public void Reset()
    {
        _revealed = false;
    }

    #endregion
}


[System.Serializable]
public class AffectOfferedItems : EffectCard
{
    [InfoBox("This EffectCard will search for all player offered items that satisfy the Item Conditions and modify them according to Item Actions. " +
        "\n\nRuns after the player offers items.")]

    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemCondition> ItemConditions;

    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemAction> ItemActions;


    private List<InventoryCardData> _matchingItems = new List<InventoryCardData>();


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

    public AffectOfferedItems()
    {
        activationTime = ActivationTime.AfterOffer;
    }
}


#region ======== [ IActions ] ========

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