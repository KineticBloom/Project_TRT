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
    [SerializeField, Tooltip("Short Text for Corners")]
    public string Text;
    [SerializeField, Tooltip("Image associated with the Condition required")]
    public Sprite ConditionImage;

    protected ActivationTime activationTime = ActivationTime.AfterOffer;

    public UnityAction OnRevealed;

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
    /// <param name="barteringController">The bartering controller to get info from</param>
    /// <param name="activationTime">When the activation is being attempted</param>
    /// <returns>Whether a boolean of whether </returns>
    public abstract bool DoesActivate(TradeInfo tradeInfo, ActivationTime activationTime);


    /// <summary>
    /// Activates the effect card
    /// </summary>
    /// <param name="barteringController">The bartering controller to modify info on</param>
    public abstract int Activate(TradeInfo tradeInfo);


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