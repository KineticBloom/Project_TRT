using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.ComponentModel;
using NaughtyAttributes;
using static EffectCard;

public class BarteringController : MonoBehaviour
{
    public bool IsActive => gameObject.activeSelf;

    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Player Dependencies")]
    public TMP_Text PlayerValueText;
    [SerializeReference]
    public List<InventoryCardObject> PlayerOfferSlots;

    [Header("NPC Dependencies")]
    public TMP_Text NPCValueText;
    public Image NPCProfilePicture;
    public InventoryCardObject NPCOfferSlotOne;

    [Header("End State Dependencies")]
    public GameObject FailBarterIcon;
    public GameObject PassBarterIcon;
    public GameObject ArrowIcon;
    public TMP_Text EndMessage;
    public GameObject EndMessageSpeechBubble;

    [Header("Effect Card Dependencies")]
    public GameObject EffectCardPrefab;
    public Transform EffectCardsContainer;

    [Header("Card Reveal Dependencies")]
    public GameObject CardRevealScreen;
    public Transform CardRevealContainer;
    public GameObject RevealEffectCardPrefab;

    [Header("Other Dependencies")]
    public Button OfferTradeButton;
    public Button ExitEarlyButton;
    public Button ExitFinalTradeButton;
    public InventoryGridController InventoryGrid;
    public InventoryBar inventoryBar;
    public GameObject targetEffectCard;

    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========
    private OfferedItems _offeredItems;
    private TradeInfo _tradeInfo;
    private List<EffectCard> _revealedEffectCards;
    private bool BarterEnding = false;

    private class TempTradeData
    {

        // Unique Dependencies
        public NPCData NPCData;
        public NpcInteractable NPCInstance;
        public InventoryCardData TargetCard;

        // Trade Data
        public float PlayerSumValue = 0;
        public int TradeAttemptsLeft = 0;
        public bool WonBarterFlag = false;
        public bool ConfirmExit = false;
    }

    TempTradeData tempTradeData;

    #endregion

    #region ======== [ INIT CALL ] ========

    public void InitializeTrade(NPCData NPCData, NpcInteractable NPCInstance, InventoryCardData TargetCard)
    {
        // Pause TimeLoop
        if (TimeLoopManager.Instance != null)
        {
            TimeLoopManager.SetLoopPaused(true);
        }

        // Load temp data
        tempTradeData = new TempTradeData();
        tempTradeData.NPCData = NPCData;
        tempTradeData.NPCInstance = NPCInstance;
        tempTradeData.TargetCard = TargetCard;

        tempTradeData.TradeAttemptsLeft = NPCData.BarterAttempts;

        // Setup
        ResetGlobalState();
        VISUAL_LoadDefault();
        EFFECT_CreateEffectCards();

        // Activate Pre-Barter Effect Cards
        EFFECT_ActivateEffectCards(ActivationTime.BeforeOffer);

        // Extra things I don't know how to refactor
        _tradeInfo = new()
        {
            OfferedItems = _offeredItems,
            ReceivedItem = tempTradeData.TargetCard
        };

        // Allow player to interact with barter!
        SetInteractable(true);
        inventoryBar.SetActiveSource(gameObject, true);
    }

    #endregion

    #region ======== [ INPUT METHODS ] ========

    public void OfferItem(InventoryCardData itemToOffer)
    {
        // Check if we can offer an item
        if (BarterEnding || itemToOffer == null || _offeredItems.Count >= 4) return;

        // Offer card
        _offeredItems.Add(itemToOffer);

        VISUAL_DisplayNewOffer();
        VISUAL_FindAndDisplayNewSum();

        // Pre-Activate any needed Effect Cards
        StartCoroutine(EFFECT_ActivateEffectCards(ActivationTime.AfterOffer, true, true));
    }

    public void RetractItem(InventoryCardData itemToRetract)
    {
        // Check if we can retract item
        if (BarterEnding || itemToRetract == null || _offeredItems.Count <= 0) return;

        // Remove effect card changes from item
        itemToRetract.ResetCurrentValue();

        // Retract item
        _offeredItems.Remove(itemToRetract);

        VISUAL_DisplayNewOffer();
        VISUAL_FindAndDisplayNewSum();

        StartCoroutine(EFFECT_ActivateEffectCards(ActivationTime.AfterOffer, true, true));
    }

    public void LeaveBarter()
    {
        LeaveScene();
    }

    public void SubmitOffer()
    {
        SetInteractable(false);
        BarterEnding = true;

        // Start process
        StartCoroutine(FinishBarter());
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    private void ResetGlobalState()
    {
        _offeredItems = new OfferedItems();
        _revealedEffectCards = new List<EffectCard>();
        BarterEnding = false;
    }
    IEnumerator FinishBarter()
    {
        // Reset all item values
        foreach (InventoryCardData x in _offeredItems.Items)
        {
            x.ResetCurrentValue();
        }

        VISUAL_DisplayNewOffer();
        VISUAL_FindAndDisplayNewSum();

        // Activate Effect Cards, wait till done!
        yield return StartCoroutine(EFFECT_ActivateEffectCards(ActivationTime.AfterOffer));

        // Check for win
        tempTradeData.WonBarterFlag = tempTradeData.PlayerSumValue >= tempTradeData.TargetCard.CurrentValue;

        if (tempTradeData.WonBarterFlag)
        {
            BarterWin();
        }
        else
        {
            BarterLose();
        }

        // Switch buttons
        OfferTradeButton.gameObject.SetActive(false);
        ExitEarlyButton.gameObject.SetActive(false);
        SetInteractable(true);
        ExitFinalTradeButton.Select();

        if (tempTradeData.TradeAttemptsLeft > 1 || tempTradeData.TradeAttemptsLeft == -1)
        {
            ExitFinalTradeButton.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1.5f);
        ExitFinalTradeButton.gameObject.SetActive(true);

        if (tempTradeData.WonBarterFlag == false)
        {

            if (tempTradeData.TradeAttemptsLeft == -1)
            {
                // Show effect cards revealed -> then restart
                StartCoroutine(EFFECT_ShowEffectCards(RestartBarter));
                yield break;
            }

            // Update attempts
            tempTradeData.TradeAttemptsLeft -= 1;

            // Try to continue
            if (tempTradeData.TradeAttemptsLeft > 0)
            {
                // Show effect cards revealed -> then restart
                StartCoroutine(EFFECT_ShowEffectCards(RestartBarter));
                yield break;
            }
        }

        StartCoroutine(EFFECT_ShowEffectCards(null));
    }

    private void BarterWin()
    {
        // Show end message
        EndMessageSpeechBubble.SetActive(true);
        EndMessage.text = tempTradeData.NPCData.BarterMessageWin;

        // Update visuals
        PassBarterIcon.SetActive(true);
        ArrowIcon.SetActive(false);

        // Remove Item from NPC
        tempTradeData.NPCInstance.ItemsAvailable.Remove(tempTradeData.TargetCard);
        if (tempTradeData.NPCInstance.ItemsAvailable.Count == 0)
        {
            GameManager.FlagTracker.SetFlag(tempTradeData.NPCData.FlagID);
        }
    }
    private void BarterLose()
    {
        // Show end message
        EndMessageSpeechBubble.SetActive(true);
        EndMessage.text = tempTradeData.NPCData.BarterMessageLose;

        // Update visuals
        FailBarterIcon.SetActive(true);
        ArrowIcon.SetActive(false);
    }
    private void RestartBarter()
    {
        // Remove effect card modifiers
        _offeredItems.ResetItemValues();
        tempTradeData.TargetCard.ResetCurrentValue();

        // Give cards back to player
        _offeredItems.ReturnCardsToInventory();
        _offeredItems.Items.Clear();

        // Load Default values
        ResetGlobalState();
        VISUAL_LoadDefault();
        VISUAL_FindAndDisplayNewSum();

        // Activate Pre-Barter Effect Cards
        EFFECT_ActivateEffectCards(ActivationTime.BeforeOffer);

        // Extra things I don't know how to refactor
        _tradeInfo = new()
        {
            OfferedItems = _offeredItems,
            ReceivedItem = tempTradeData.TargetCard
        };

        // Allow player to interact with barter!
        SetInteractable(true);
    }
    private void LeaveScene()
    {
        // Destroy effect cards
        foreach (Transform card in EffectCardsContainer)
        {
            Destroy(card.gameObject);
        }

        foreach (InventoryCardData x in _offeredItems.Items)
        {
            x.ResetCurrentValue();
        }

        // Remove effect card modifiers
        tempTradeData.TargetCard.ResetCurrentValue();

        // Give cards back to player
        _offeredItems.ReturnCardsToInventory();

        // Clear effect card modifications
        GameManager.Inventory.ResetAllCardValues();

        // Complete trade if won!
        if (tempTradeData.WonBarterFlag)
        {
            // Complete trade swap
            foreach (InventoryCardData card in _offeredItems.Items)
            {
                GameManager.Inventory.RemoveCard(card);
            }

            GameManager.Inventory.AddCard(tempTradeData.TargetCard);
        }

        _offeredItems.Items.Clear();

        // Leave Scene
        StartCoroutine(LeaveSceneAtFrameEnd());
    }
    private void SetInteractable(bool isInteractable)
    {
        foreach (InventoryCardObject x in PlayerOfferSlots)
        {
            x.SetInteractable(isInteractable);
        }

        OfferTradeButton.interactable = isInteractable;
        ExitEarlyButton.interactable = isInteractable;
        ExitFinalTradeButton.interactable = isInteractable;
        InventoryGrid.SetSlotsInteractable(isInteractable);
    }

    IEnumerator LeaveSceneAtFrameEnd()
    {
        yield return new WaitForEndOfFrame();
        InGameUi _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        _inGameUi.MoveToDefault();

        // Change focus
        inventoryBar.SetActiveSource(gameObject, false);

        // Unpause time
        if (TimeLoopManager.Instance != null) TimeLoopManager.SetLoopPaused(false);
    }

    #endregion

    #region ======== [ VISUAL PRIVATE METHODS ] ========

    private void VISUAL_FindAndDisplayNewSum()
    {
        tempTradeData.PlayerSumValue = 0;

        foreach (InventoryCardData item in _offeredItems.Items)
        {
            tempTradeData.PlayerSumValue += item.CurrentValue;
        }

        PlayerValueText.text = "Value: " + tempTradeData.PlayerSumValue + "?";
    }
    private void VISUAL_DisplayNewOffer()
    {
        List<InventoryCardData> CardsLeftToDisplay = _offeredItems.Items;
        int index = 0;

        /*
         * If we have a card to display, show it!
         * Otherwise, set to empty.
         */
        foreach (InventoryCardObject x in PlayerOfferSlots)
        {
            if (index < CardsLeftToDisplay.Count)
            {
                x.SetData(CardsLeftToDisplay[index], x.IsPreviewCard);
                index += 1;
            }
            else
            {
                x.SetCardToEmpty(x.IsPreviewCard);
            }
        }
    }
    private void VISUAL_LoadDefault()
    {
        // Reset slot contents
        NPCOfferSlotOne.SetData(tempTradeData.TargetCard, false);

        foreach (InventoryCardObject x in PlayerOfferSlots)
        {
            x.SetCardToEmpty(false);
        }

        //  Hide end popups
        FailBarterIcon.SetActive(false);
        PassBarterIcon.SetActive(false);
        EndMessageSpeechBubble.SetActive(false);

        // Init buttons
        OfferTradeButton.gameObject.SetActive(true);
        ExitEarlyButton.gameObject.SetActive(true);
        ExitFinalTradeButton.gameObject.SetActive(false);

        ArrowIcon.SetActive(true);

        // Reset value texts
        PlayerValueText.text = "Value: 0";
        NPCValueText.text = "Value: " + tempTradeData.TargetCard.CurrentValue;

        // Load Picture of NPC
        NPCProfilePicture.sprite = tempTradeData.NPCData.Icon;
    }

    #endregion

    #region ======== [ EFFECT CARD METHODS ] ========

    public delegate void EFFECT_CallBack();

    /// <summary>
    /// Resets the card Reveal Screen and Close it
    /// </summary>
    public void EFFECT_CloseCardScreen()
    {
        CardRevealScreen.SetActive(false);

        foreach (Transform card in CardRevealContainer)
        {
            Destroy(card.gameObject);
        }
    }
    public void EFFECT_AddNewReveal(EffectCard effectCard)
    {
        if (_revealedEffectCards == null) return;

        _revealedEffectCards.Add(effectCard);
    }
    private void EFFECT_CreateEffectCards()
    {
        // Init each effect card inside a EffectCardDisplay instance
        foreach (EffectCard effectCard in tempTradeData.NPCData.EffectCards)
        {
            GameObject card = Instantiate(EffectCardPrefab, EffectCardsContainer);
            card.GetComponent<EffectCardDisplay>().Load(effectCard, this);
        }
    }
    IEnumerator EFFECT_ActivateEffectCards(ActivationTime activationTime, bool SkipFlipDelay = false, bool PreActivation = false)
    {
        List<EffectCard> effectCards = tempTradeData.NPCData.EffectCards;
        List<EffectCard> activeEffectCards = new List<EffectCard>();

        // Get list of cards to activate
        foreach (EffectCard effectCard in effectCards)
        {
            if (PreActivation == false || (PreActivation && effectCard.IsRevealed))
            {
                if (effectCard.DoesActivate(_tradeInfo, activationTime))
                {
                    activeEffectCards.Add(effectCard);
                }
            }
        }

        float flipDelay = 1f;
        if (SkipFlipDelay) flipDelay = 0f;

        // Reset current values
        foreach (InventoryCardData x in _offeredItems.Items)
        {
            x.ResetCurrentValue();
        }

        foreach (InventoryCardData x in GameManager.Inventory.Get())
        {
            x.ResetCurrentValue();
        }

        yield return new WaitForSeconds(flipDelay);

        InventoryCardData lastCardAdded = null;

        if (_offeredItems.Count != 0)
        {
            lastCardAdded = _offeredItems.Items[_offeredItems.Count - 1];
        }


        // Flip each card that we can activate
        foreach (EffectCard effectCard in activeEffectCards)
        {
            bool skipAnimation = false;
            if (PreActivation && lastCardAdded != null)
            {
                skipAnimation = effectCard.DoesActivate(lastCardAdded, _tradeInfo, activationTime) == false;
            }

            bool WillFlip = effectCard.IsRevealed == false;

            effectCard.Activate(_tradeInfo, SkipFlipDelay == false, skipAnimation);

            if (skipAnimation == false && WillFlip)
            {
                yield return new WaitForSeconds(1f); // Activate should be a IEnumerator that we wait for.
            }

            VISUAL_DisplayNewOffer();
            VISUAL_FindAndDisplayNewSum();

            yield return new WaitForSeconds(flipDelay);
        }
    }
    private WaitForCloseRevealScreen EFFECT_ShowRevealedCards()
    {
        CardRevealScreen.SetActive(true);

        foreach (EffectCard effectCard in _revealedEffectCards)
        {
            GameObject effectCardDisplay = Instantiate(RevealEffectCardPrefab, CardRevealContainer);

            effectCardDisplay.GetComponent<EffectCardDisplay>().Load(effectCard, null);
        }

        _revealedEffectCards.Clear();

        return new WaitForCloseRevealScreen(CardRevealScreen);
    }
    private IEnumerator EFFECT_ShowEffectCards(EFFECT_CallBack x)
    {
        if (_revealedEffectCards.Count > 0)
        {
            yield return EFFECT_ShowRevealedCards();
        }

        x?.Invoke();
    }

    #endregion
}

//// SCARY STUFF BELOW THIS! (Jonah doesn't know what it does)

[System.Serializable]
public class OfferedItems
{
    public List<InventoryCardData> Items;
    public int Count { get { return Items.Count; } }


    public OfferedItems()
    {
        Items = new List<InventoryCardData>();
    }

    public void ResetItemValues()
    {
        foreach (InventoryCardData x in Items)
        {
            x.ResetCurrentValue();
        }
    }

    public void Add(InventoryCardData card)
    {
        Items.Add(card);

        GameManager.Inventory.RemoveCard(card, true);
    }

    public void AddNoLoss(InventoryCardData card)
    {
        Items.Add(card);
    }

    public void Remove(InventoryCardData card)
    {
        Items.Remove(card);
        GameManager.Inventory.AddCard(card, true);
    }

    public void ReturnCardsToInventory()
    {
        foreach (InventoryCardData card in Items)
        {
            GameManager.Inventory.AddCard(card, true);
        }
    }
}

public struct TradeInfo
{
    public OfferedItems OfferedItems;
    public InventoryCardData ReceivedItem;
}

public class WaitForCloseRevealScreen : CustomYieldInstruction
{
    private GameObject _revealScreen;

    public override bool keepWaiting
    {
        get
        {
            return _revealScreen.activeInHierarchy;
        }
    }

    public WaitForCloseRevealScreen(GameObject revealScreen)
    {
        _revealScreen = revealScreen;
    }
}
