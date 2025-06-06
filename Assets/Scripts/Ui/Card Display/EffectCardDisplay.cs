using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;
using DG.Tweening;

public class EffectCardDisplay : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject cardBack;

    [Header("Card Front")]
    [SerializeField] private GameObject cardFront;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI topLeftText;
    [SerializeField] private Image backArt;
    [SerializeField] private Sprite backActive;
    [SerializeField] private Sprite backNotActive;

    [Header("Description")]
    [SerializeField] private GameObject descriptionContainer;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Parameters")]
    [SerializeField] private bool revealScreen = false;
    [SerializeField] private float flipBackDuration = 0.15f;
    [SerializeField] private Ease flipBackEase = Ease.InQuad;
    [SerializeField] private float flipFrontDuration = 0.15f;
    [SerializeField] private Ease flipFrontEase = Ease.OutExpo;
    [SerializeField] private float flipMoveHeight = 25f;
    [SerializeField] private Ease flipFrontMoveEase = Ease.InExpo;
    [SerializeField] private Ease flipBackMoveEase = Ease.OutExpo;

    [HideIf("InPlayMode")]
    public EffectCard EffectCard;

    private Tweener _shakeTween = null;
    private BarteringController _barteringController;
    private float _baseScale = 1f;
    private float _baseHeight;
    private Vector3 _returnPoint;
    private float AttackSpeed = 0.125f;
    private Vector3 defaultUp;
    private bool flipDone = false;
    private bool flipInProgress = false;

    /// <summary>
    /// Loads the Effect Card to be displayed
    /// </summary>
    /// <param name="effectCard"></param>
    public void Load(EffectCard effectCard, BarteringController barteringController)
    {
        EffectCard = effectCard;

        icon.sprite = effectCard.Icon;
        topLeftText.text = effectCard.Text;
/*        bottomRightText.text = effectCard.Text;
        conditionImageTop.sprite = effectCard.ConditionImage;
        conditionImageBottom.sprite = effectCard.ConditionImage;*/

        descriptionText.text = (revealScreen || effectCard.IsRevealed)
            ? effectCard.Description : effectCard.Hint;

        cardBack.SetActive(revealScreen || !effectCard.IsRevealed);
        cardFront.SetActive(!revealScreen && effectCard.IsRevealed);

        if (revealScreen)
        {
            Reveal();
        }
        else
        {
            HideDescription();
            _barteringController = barteringController;
            effectCard.OnRevealed += Reveal;
            effectCard.OnActivate += Activate;
            effectCard.OnAttackActivate += AttackActivate;
        }

        _baseScale = cardFront.transform.localScale.x;

        flipDone = effectCard.IsRevealed;
        flipInProgress = false;
    }


    /// <summary>
    /// Shows the front of the card and hides the back
    /// </summary> 
    [Button]
    public void Reveal()
    {
        if (flipInProgress == true || flipDone == true) return;

        backArt.sprite = backNotActive;

        _barteringController?.EFFECT_AddNewReveal(EffectCard);
        flipInProgress = true;
        FlipBack();
    }

    public void Activate()
    {
        //transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
        if (_shakeTween == null)
        {
            backArt.sprite = backActive;
            _shakeTween = transform.DOShakeRotation(0.5f, new Vector3(0, 0, 45f), 15, 90, true, ShakeRandomnessMode.Harmonic);
            _shakeTween.SetAutoKill(false);
        }
        else
        {
            _shakeTween.Restart();
        }
        PlayerInputHandler.SetHaptics(0.5f, 0, 0.1f);
    }

    public void AttackActivate()
    {
        StartCoroutine(AttackActivateAnim());
    }

    IEnumerator AttackActivateAnim()
    {

        if (flipDone == false && flipInProgress == false)
        {
            Reveal();
        }

        bool Flipping = flipInProgress;

        yield return new WaitUntil(CheckForFlip);

        if (Flipping)
        {
            yield return new WaitForSeconds(0.5f);
        }

        _returnPoint = transform.position;
        Vector3 target = _barteringController.targetEffectCard.transform.position;
        transform.DOMove(target, AttackSpeed, false).OnComplete(AttackReturnCall);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), AttackSpeed).SetEase(Ease.OutElastic);
        defaultUp = transform.up;
        transform.up = target - transform.position;
    }

    private bool CheckForFlip()
    {
        return flipDone;
    }

    public void AttackReturnCall()
    {
        backArt.sprite = backActive;
        StartCoroutine(AttackReturn());
        PlayerInputHandler.SetHaptics(1f, 1f, 0.1f);
    }

    IEnumerator AttackReturn()
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 target = _returnPoint;
        transform.DOMove(_returnPoint, AttackSpeed * 1.5f, false).SetEase(Ease.OutExpo).OnComplete(ResetRotation);
        transform.up = target - transform.position;
    }

    private void ResetRotation()
    {
        DOTween.To(() => transform.up, x => transform.up = x, defaultUp, 0.2f);
    }

    /// <summary>
    /// First part of the flipping animation
    /// </summary>
    private void FlipBack()
    {
        _baseScale = cardFront.transform.localScale.x;
        _baseHeight = cardFront.transform.localPosition.y;

        cardBack.transform.localScale = Vector3.one * _baseScale;
        cardBack.transform.localPosition = new Vector3(
            cardFront.transform.localPosition.x,
            _baseHeight,
            cardFront.transform.localPosition.z);

        cardBack.SetActive(true);
        cardFront.SetActive(false);


        cardBack.transform.DOLocalMoveY(_baseHeight + flipMoveHeight, flipBackDuration)
            .SetEase(flipBackMoveEase);
        cardBack.transform.DOScaleX(0, flipBackDuration)
            .SetEase(flipBackEase)
            .OnComplete(() => FlipFront());
    }


    /// <summary>
    /// Second part of the flipping animation
    /// </summary>
    private void FlipFront()
    {
        PlayerInputHandler.SetHaptics(0, 0.5f, 0.5f);
        cardFront.transform.localScale = Vector3.up * _baseScale;
        cardFront.transform.localPosition = new Vector3(
            cardFront.transform.localPosition.x,
            _baseHeight + flipMoveHeight,
            cardFront.transform.localPosition.z);

        cardBack.SetActive(false);
        cardFront.SetActive(true);

        cardFront.transform.DOLocalMoveY(_baseHeight, flipFrontDuration)
            .SetEase(flipFrontMoveEase);
        cardFront.transform.DOScaleX(_baseScale, flipFrontDuration)
            .SetEase(flipFrontEase).OnComplete(() => { flipDone = true; flipInProgress = false; });
    }


    /// <summary>
    /// Shows the card's description if revealed
    /// </summary>
    public void ShowDescription()
    {
        if (descriptionText.text.Length == 0) return;

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
        backArt.sprite = backNotActive;
        transform.DOKill();

        if (_shakeTween != null)
        {
            _shakeTween.Kill();
            _shakeTween = null;
        }
        if (!revealScreen)
        {
            EffectCard.OnRevealed -= Reveal;
            EffectCard.OnActivate -= Activate;
            EffectCard.OnAttackActivate -= AttackActivate;
        }
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
