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