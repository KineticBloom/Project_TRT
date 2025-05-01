using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.ComponentModel;
using NaughtyAttributes;

public class BarteringController : MonoBehaviour {

    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Player Dependencies")]
    public TMP_Text PlayerValueText;
    public InventoryCardObject PlayerOfferSlotOne;
    public InventoryCardObject PlayerOfferSlotTwo;
    public InventoryCardObject PlayerOfferSlotThree;
    public InventoryCardObject PlayerOfferSlotFour;

    [Header("NPC Dependencies")]
    public TMP_Text NPCValueText;
    public Image NPCProfilePicture;
    public InventoryCardObject NPCOfferSlotOne;

    [Header("End State Dependencies")]
    public GameObject FailBarterIcon;
    public GameObject PassBarterIcon;
    public TMP_Text EndMessage;
    public GameObject EndMessageSpeechBubble;

    [Header("Effect Card Dependencies")]
    public GameObject EffectCardPrefab;
    public Transform EffectCardsContainer;

    [Header("Other Dependencies")]
    public Button OfferTradeButton;
    public InventoryGridController InventoryGrid;
    public InventoryBar inventoryBar;

    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========

    private NPCData _currentNPCData;
    private float _currentOfferedValue = 0;
    private bool _wonBarter = false;
    private InventoryCardObject _currentButtonObject;
    private OfferedItems _offeredItems;
    private int _currentAttempts = 0;
    private TradeInfo _tradeInfo;

    #endregion

    /*
     * Sequence of events
     * 
     * InitializeTrade
     * Activate Pre-Barter Effect Cards
     * Player offers items
     * Submit offer -> EndBarter
     * Activate Post-Barter Effect cards
     * Win? -> Trade items and end barter
     * Lose? -> Restart to beginning of sequence
     * 
     */


    #region ======== [ INIT METHOD ] ========

    /// <summary>
    /// Start a barter for a given item.
    /// </summary>
    /// <param name="npcData">Information about the trade with this NPC.</param>
    public void InitializeTrade(NPCData npcData, bool firstTime = true) {

        TimeLoopManager.SetLoopPaused(true);
        // Setup trackers
        _currentNPCData = npcData;
        _offeredItems = new OfferedItems();

        // Init new barter
        ResetData();

        // Load NPC Data
        NPCOfferSlotOne.SetData(_currentNPCData.ItemOnOffer, false);
        NPCValueText.text = "Value: " + _currentNPCData.ItemOnOffer.CurrentValue;
        NPCProfilePicture.sprite = _currentNPCData.Icon;


        // Only runs the first time
        if (firstTime)
        {
            _currentAttempts = 0;

            // Load Effect Cards
            foreach (EffectCard effectCard in _currentNPCData.EffectCards)
            {
                GameObject card = Instantiate(EffectCardPrefab, EffectCardsContainer);
                card.GetComponent<EffectCardDisplay>().Load(effectCard);
            }

            // Activate Pre-Barter Effect Cards
            ActivateEffectCards(EffectCard.ActivationTime.BeforeOffer);
        }

        _tradeInfo = new()
        {
            OfferedItems = _offeredItems,
            ReceivedItem = _currentNPCData.ItemOnOffer,
        };

        SetInteractable(true);

        UpdateVisuals();
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========


    /// <summary>
    /// Returns whether the bartering interface is showing or not
    /// </summary>
    public bool IsActive => gameObject.activeSelf;

    /// <summary>
    /// Offer a item to the barter pool.
    /// Called by UI Elements.
    /// </summary>
    /// <param name="itemToOffer">Item to offer to the offer pool.</param>
    public void OfferItem(InventoryCardData itemToOffer) {

        if (itemToOffer == null) return;

        if (_offeredItems.Count >= 4) return;

        // See what button was activated
        _currentButtonObject = null;

        // Get Current object of selected InventoryGridController
        _currentButtonObject = InventoryGrid.FindCurrentSelection();

        _offeredItems.Add(itemToOffer);

        UpdateVisuals();

        // Reset selection of button!
        if (_currentButtonObject != null) {
            _currentButtonObject.CurrentActiveButton.Select();
        }
    }

    /// <summary>
    /// Remove a item from the current barter pool.
    /// Called by UI Elements.
    /// </summary>
    /// <param name="itemToRemove">Item to retract from the offer pool.</param>
    public void RetractItem(InventoryCardData itemToRemove) {

        if (itemToRemove == null) return;

        // See what button was activated
        _currentButtonObject = null;

        if (PlayerOfferSlotOne.CurrentActiveButton.gameObject == EventSystem.current.currentSelectedGameObject) {
            _currentButtonObject = PlayerOfferSlotOne;
        }
        if (PlayerOfferSlotTwo.CurrentActiveButton.gameObject == EventSystem.current.currentSelectedGameObject) {
            _currentButtonObject = PlayerOfferSlotTwo;
        }
        if (PlayerOfferSlotThree.CurrentActiveButton.gameObject == EventSystem.current.currentSelectedGameObject)
        {
            _currentButtonObject = PlayerOfferSlotThree;
        }
        if (PlayerOfferSlotFour.CurrentActiveButton.gameObject == EventSystem.current.currentSelectedGameObject)
        {
            _currentButtonObject = PlayerOfferSlotFour;
        }

        // Remove item
        _offeredItems.Remove(itemToRemove);

        UpdateVisuals();

        // Reset selection of button!
        if (_currentButtonObject != null) {
            _currentButtonObject.CurrentActiveButton.Select();
        }
    }

    /// <summary>
    /// Leave and fail barter.
    /// </summary>
    public void LeaveBarter() {
        StartCoroutine(LeaveBarterScene());
    }

    /// <summary>
    /// End barter and determines if player pool is valuable enough for NPC.
    /// Called by UI Elements.
    /// </summary>
    public void EndBarter() {

        SetInteractable(false);

        ActivateEffectCards(EffectCard.ActivationTime.AfterOffer);

        float NPCItemValue = _currentNPCData.ItemOnOffer.CurrentValue;

        EndMessageSpeechBubble.SetActive(true);

        if (_currentOfferedValue >= NPCItemValue) {
            // Complete Trade
            EndMessage.text = _currentNPCData.BarterMessageWin;
            PassBarterIcon.SetActive(true);
            GameManager.FlagTracker.SetFlag(_currentNPCData.FlagID);
            _wonBarter = true;

            StartCoroutine(LeaveBarterScene());
        } else {
            // Say no!
            EndMessage.text = _currentNPCData.BarterMessageLose;
            FailBarterIcon.SetActive(true);


            // If you run out of attempts, the barter is exited
            var barterAttempts = _currentNPCData.BarterAttempts;
            _currentAttempts++;

            if (barterAttempts <= _currentAttempts && barterAttempts > 0)
            {
                StartCoroutine(LeaveBarterScene());
                return;
            }

            // if you failed the barter, it is interactable so you can try again
            StartCoroutine(RestartBarter());
        }
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Activates Effect Cards
    /// </summary>
    /// <param name="isPreBarter">which stage of the barter are we in for activating the effect cards?</param>
    private void ActivateEffectCards(EffectCard.ActivationTime activationTime)
    {
        List<EffectCard> effectCards = _currentNPCData.EffectCards;
        List<EffectCard> activeEffectCards = new List<EffectCard>();

        foreach (EffectCard effectCard in effectCards)
        {
            if (effectCard.DoesActivate(_tradeInfo, activationTime))
            {
                activeEffectCards.Add(effectCard);
            }
        }

        foreach (EffectCard effectCard in activeEffectCards)
        {
            effectCard.Activate(_tradeInfo);
        }

        UpdateVisuals();
    }

    private void UpdateVisuals() {

        ResetPlayerData();

        // Get new player offer value
        foreach (InventoryCardData item in _offeredItems.Items) {
            _currentOfferedValue += item.CurrentValue;
        }

        PlayerValueText.text = "Value: " + _currentOfferedValue;

        // Display new slots adjusted
        if (_offeredItems.Count >= 1) {
            PlayerOfferSlotOne.SetData(_offeredItems.Items[0], PlayerOfferSlotOne.IsPreviewCard);
        }
        if (_offeredItems.Count >= 2) {
            PlayerOfferSlotTwo.SetData(_offeredItems.Items[1], PlayerOfferSlotTwo.IsPreviewCard);
        }
        if (_offeredItems.Count >= 3)
        {
            PlayerOfferSlotThree.SetData(_offeredItems.Items[2], PlayerOfferSlotThree.IsPreviewCard);
        }
        if (_offeredItems.Count >= 4)
        {
            PlayerOfferSlotFour.SetData(_offeredItems.Items[3], PlayerOfferSlotFour.IsPreviewCard);
        }

        NPCValueText.text = "Value: " + _currentNPCData.ItemOnOffer.CurrentValue;

        inventoryBar.SetActiveSource(gameObject, true);
    }

    private void ResetData() {

        ResetPlayerData();
        ResetNPCData();

        // Hide end objects.
        FailBarterIcon.SetActive(false);
        PassBarterIcon.SetActive(false);
        EndMessageSpeechBubble.SetActive(false);

        // Reset trackers
        _wonBarter = false;
    }

    private void ResetPlayerData() {
        PlayerOfferSlotOne.SetCardToEmpty(false);
        PlayerOfferSlotTwo.SetCardToEmpty(false);
        PlayerOfferSlotThree.SetCardToEmpty(false);
        PlayerOfferSlotFour.SetCardToEmpty(false);
        _currentOfferedValue = 0;
        PlayerValueText.text = "Value: 0";
    }

    private void ResetNPCData() {
        NPCOfferSlotOne.SetCardToEmpty(false);
        NPCValueText.text = "Value: 0";
    }

    IEnumerator RestartBarter()
    {
        yield return new WaitForSeconds(1f);

        foreach (var item in _offeredItems.Items)
        {
            item.ResetCurrentValue();
        }
        _currentNPCData.ItemOnOffer.ResetCurrentValue();

        _offeredItems.ReturnCardsToInventory();
        _offeredItems.Items.Clear();
        // GameManager.Inventory.ResetAllCardValues();

        InitializeTrade(_currentNPCData, false);
    }

    IEnumerator LeaveBarterScene() {
        yield return new WaitForSeconds(1f);

        foreach (var item in _offeredItems.Items)
        {
            item.ResetCurrentValue();
        }

        _offeredItems.ReturnCardsToInventory();

        if (_wonBarter) {
            // remove cards offered
            foreach (InventoryCardData card in _offeredItems.Items)
            {
                GameManager.Inventory.RemoveCard(card);
            }

            GameManager.Inventory.AddCard(_currentNPCData.ItemOnOffer);
        }

        _offeredItems = null;

        // Remove Effect Cards
        foreach (Transform card in EffectCardsContainer)
        {
            Destroy(card.gameObject);
        }

        GameManager.Inventory.ResetAllCardValues();

        InGameUi _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();

        _inGameUi.MoveToDefault();
        TimeLoopManager.SetLoopPaused(false);

        inventoryBar.SetActiveSource(gameObject, false);
    }

    /// <summary>
    /// Sets whether or not the player can make any inputs
    /// </summary>
    /// <param name="isInteractable"></param>
    private void SetInteractable(bool isInteractable)
    {
        PlayerOfferSlotOne.SetInteractable(isInteractable);
        PlayerOfferSlotTwo.SetInteractable(isInteractable);
        PlayerOfferSlotThree.SetInteractable(isInteractable);
        PlayerOfferSlotFour.SetInteractable(isInteractable);

        OfferTradeButton.interactable = isInteractable;
        InventoryGrid.SetSlotsInteractable(isInteractable);
    }

    #endregion
}

[System.Serializable]
public class OfferedItems
{
    public List<InventoryCardData> Items;
    public int Count {  get { return Items.Count; } }


    public OfferedItems()
    {
        Items = new List<InventoryCardData>();
    }

    public void Add(InventoryCardData card)
    {
        Items.Add(card);
        GameManager.Inventory.RemoveCard(card, true);
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