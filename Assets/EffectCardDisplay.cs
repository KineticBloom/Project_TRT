using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EffectCardDisplay : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject cardBack;
    [SerializeField] private GameObject cardFront;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI topLeftText;
    [SerializeField] private TextMeshProUGUI bottomRightText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// Loads the Effect Card to be displayed
    /// </summary>
    /// <param name="effectCard"></param>
    public void Load(EffectCard effectCard)
    {
        icon.sprite = effectCard.Icon;
        topLeftText.text = effectCard.Text;
        bottomRightText.text = effectCard.Text;

        cardBack.SetActive(!effectCard.IsRevealed);
        cardFront.SetActive(effectCard.IsRevealed);
    }


    /// <summary>
    /// Reveals the Card
    /// </summary>
    public void Reveal()
    {
        cardBack.SetActive(false);
        cardFront.SetActive(true);
    }
}
