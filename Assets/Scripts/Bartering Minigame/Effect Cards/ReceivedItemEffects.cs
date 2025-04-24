using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
#if UNITY_EDITOR
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed
#endif

[System.Serializable]
public class ReceivedItemEffects : EffectCard
{
    #region ======== [ PARAMETERS ] ========

    [InfoBox("This EffectCard will search for all player offered items and other conditions that satisfy the Item Conditions and modify the received according to Item Actions. " +
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
    public override bool DoesActivate(TradeInfo tradeInfo, ActivationTime activationTime)
    {
        if (this.activationTime != activationTime && activationTime != ActivationTime.Both) return false;

        _matchingItems.Clear();

        foreach (InventoryCardData item in tradeInfo.OfferedItems.Items)
        {
            bool addItem = false;

            foreach (IItemCondition condition in ItemConditions)
            {
                if (condition.IsSatisfied(item, tradeInfo))
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
    public override void Activate(TradeInfo tradeInfo)
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
    public ReceivedItemEffects()
    {
        activationTime = ActivationTime.AfterOffer;
    }

    #endregion
}
