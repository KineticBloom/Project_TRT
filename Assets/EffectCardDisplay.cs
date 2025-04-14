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
    /// Reveals the Card
    /// </summary>
    public void Reveal()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
    }


    public void ShowDescription()
    {
        if (descriptionText.text.Length == 0) return;
        if (!EffectCard.IsRevealed) return;

        descriptionContainer.SetActive(true);
    }


    public void HideDescription()
    {
        descriptionContainer.SetActive(false);
    }


    private void OnDisable()
    {
        EffectCard.OnRevealed -= Reveal;
    }


    private bool InPlayMode()
    {
        return Application.isPlaying;
    }
}
