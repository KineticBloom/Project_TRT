using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class EffectCardDisplay : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject cardBack;
    [SerializeField] private GameObject cardFront;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI topLeftText;
    [SerializeField] private TextMeshProUGUI bottomRightText;
    [SerializeField] private GameObject descriptionContainer;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [HideIf("InPlayMode")]
    public EffectCard EffectCard;


    /// <summary>
    /// Loads the Effect Card to be displayed
    /// </summary>
    /// <param name="effectCard"></param>
    public void Load(EffectCard effectCard)
    {
        EffectCard = effectCard;

        icon.sprite = effectCard.Icon;
        topLeftText.text = effectCard.Text;
        bottomRightText.text = effectCard.Text;

        cardBack.SetActive(!effectCard.IsRevealed);
        cardFront.SetActive(effectCard.IsRevealed);

        descriptionText.text = effectCard.Description;

        HideDescription();

        effectCard.OnRevealed += Reveal;
    }


    /// <summary>
    /// Shows the front of the card and hide the back
    /// </summary>
    public void Reveal()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
    }


    /// <summary>
    /// Shows the card's description if revealed
    /// </summary>
    public void ShowDescription()
    {
        if (descriptionText.text.Length == 0) return;
        if (!EffectCard.IsRevealed) return;

        descriptionContainer.SetActive(true);
    }


    /// <summary>
    /// Hides the card's description
    /// </summary>
    public void HideDescription()
    {
        descriptionContainer.SetActive(false);
    }


    private void OnDisable()
    {
        EffectCard.OnRevealed -= Reveal;
    }


    /// <summary>
    /// Used by "EffectCard" to check if the game is playing to be revealed
    /// </summary>
    /// <returns></returns>
    private bool InPlayMode()
    {
        return Application.isPlaying;
    }
}
