using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using System.Linq;


#if UNITY_EDITOR
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed
#endif

[System.Serializable]
public class OfferedItemsEffects : EffectCard
{
    #region ======== [ PARAMETERS ] ========

    [InfoBox("This EffectCard will search for all player offered items that satisfy the Item Conditions and modify them according to Item Actions. " +
        "\n\nRuns after the player offers items.")]

    [Header("Conditions")]
    [SerializeField, Tooltip("Does it require all Conditions to be met (AND) or any (OR)?")]
    public bool RequiresAllConditions = true;
    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemCondition> ItemConditions;

    [Space, Space]
    [Header("Effect Settings")]
    [SerializeField, Tooltip("If card can activate, and these items are being offered, they will be affected")]
    public List<InventoryCardData> AffectedItems = new();
    [SerializeField, Tooltip("If card can activate, and items with these tags are being offered, they will be affected")]
    public List<string> AffectedTags = new();

    [Space, Space]
    [Header("Actions")]
    [SerializeReference, SubclassSelector]
    [Tooltip("List of item actions that determine the effect of the found items")]
    public List<IItemAction> ItemActions;

    #endregion

    #region ======== [ FUNCTIONS ] ========

    /// <summary>
    /// Adds any items matching the conditions to _matchingItems
    /// </summary>
    /// <returns></returns>
    public override bool DoesActivate(TradeInfo tradeInfo, ActivationTime activationTime)
    {
        if (this.activationTime != activationTime && activationTime != ActivationTime.Both) return false;

        // Store the results of all conditions
        List<bool> conditionResults = new();
        foreach (IItemCondition condition in ItemConditions)
        {
            conditionResults.Add(condition.IsSatisfied(tradeInfo));
        }

        if (RequiresAllConditions)
        {
            _canActivate = conditionResults.All((bool condition) => condition);
        } else
        {
            _canActivate = conditionResults.Any((bool condition) => condition);
        }

        return _canActivate;
    }


    /// <summary>
    /// Apply the ItemActions to all items that are relevant to AffectedItems and AffectedTags
    /// </summary>
    public override int Activate(TradeInfo tradeInfo)
    {
        int itemsAffected = 0;

        foreach (InventoryCardData offeredItem in tradeInfo.OfferedItems.Items)
        {
            bool affected = false;
            
            foreach (InventoryCardData affectedItem in AffectedItems)
            {
                if (offeredItem.IsSame(affectedItem)) { affected = true; break; }
            }

            foreach (string affectedTag in AffectedTags)
            {
                // lowercase everything in offeredItem.Tags to avoid false negatives
                if (offeredItem.Tags.Any(tag => tag.ToLower().Equals(affectedTag))) { affected = true; break; }
            }

            if (!affected) continue;

            foreach (IItemAction action in ItemActions)
            {
                action.Activate(offeredItem);
            }

            itemsAffected++;
        }

        Reveal();

        return itemsAffected;
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
