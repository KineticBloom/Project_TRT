using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
#if UNITY_EDITOR
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed
#endif

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
    [SerializeField, Tooltip("Hint for the what the card might do?")]
    public string Hint = "What could this do?";
    [SerializeField, Tooltip("Short Text for Corners")]
    public string Text;
    [SerializeField, Tooltip("Image associated with the Condition required")]
    public Sprite ConditionImage;

    protected ActivationTime activationTime = ActivationTime.AfterOffer;

    public UnityAction OnRevealed;
    public UnityAction OnActivate;
    public UnityAction OnAttackActivate;

    #endregion

    #region ======== [ PRIVATE VARIABLE ] ========
    [SerializeField, ReadOnly]
    private bool _revealed = false;
    public bool IsRevealed {  get { return _revealed; } set { _revealed = value; } }
    protected bool _canActivate = false;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Checks whether the card can activate or not
    /// </summary>
    /// <param name="cardInfo">Information for the card</param>
    /// <param name="tradeInfo">Information for the trade</param>
    /// <param name="activationTime">When the activation is being attempted</param>
    /// <returns>Whether a boolean of whether </returns>
    public abstract bool DoesActivate(InventoryCardData cardInfo, TradeInfo tradeInfo, ActivationTime activationTime);

    /// <summary>
    /// Checks whether a card can activate or not
    /// </summary>
    /// <param name="cardData">Information of a given card</param>
    /// <param name="activationTime">When the activation is being attempted</param>
    /// <returns>Whether a boolean of whether </returns>
    public abstract bool DoesActivate(TradeInfo cardData, ActivationTime activationTime);

    /// <summary>
    /// Activates the effect card
    /// </summary>
    /// <param name="tradeInfo">Information for the card to modify</param>
    public virtual int Activate(TradeInfo tradeInfo, bool playAttackAnimation, bool skipAnimations) {

        if (skipAnimations) return 0;

        if (playAttackAnimation)
        {
            OnAttackActivate.Invoke();
        }
        else
        {
            OnActivate.Invoke();
        }
        return 0;
    }


    /// <summary>
    /// Reveal the card if not revealed already
    /// </summary>
    public void Reveal()
    {
        if (_revealed) return;

        OnRevealed?.Invoke();
        _revealed = true;
        SaveSystem.Save();
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