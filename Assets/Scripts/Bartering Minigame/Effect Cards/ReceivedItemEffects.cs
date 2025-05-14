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

    [SerializeField, Tooltip("Does it require all Conditions to be met (AND) or any (OR)?")]
    public bool RequiresAllConditions = true;

    #endregion

    #region ======== [ FUNCTIONS ] ========

    /// <summary>
    /// returns whether or not the Effect Card can Activate
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
        }
        else
        {
            _canActivate = conditionResults.Any((bool condition) => condition);
        }

        return _canActivate;
    }


    /// <summary>
    /// Apply the ItemActions to items in _matchingItems
    /// </summary>
    public override int Activate(TradeInfo tradeInfo, bool playAttackAnimation)
    {
        base.Activate(tradeInfo,playAttackAnimation);

        foreach (IItemAction action in ItemActions)
        {
            action.Activate(tradeInfo.ReceivedItem);
        }

        Reveal();

        return 1;
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
