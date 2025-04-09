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

    public struct TradeData {
        public InventoryCardData ItemOnOffer;
        public string DialogueForTrade;
        public string DialogueForNoTrade;
        public NPCData NPCData;
    }

    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========

    private TradeData _currentTradeInformation;
    private float _currentOfferedValue = 0;
    private bool _wonBarter = false;
    private InventoryCardObject _currentButtonObject;
    private OfferedItems _offeredItems;

    #endregion

    #region ======== [ INIT METHOD ] ========

    /// <summary>
    /// Start a barter for a given item.
    /// </summary>
    /// <param name="TradeInformation">Item being offered by a NPC.</param>
    public void InitializeTrade(TradeData TradeInformation) {

        // Setup trackers
        _currentTradeInformation = TradeInformation;
        _offeredItems = new OfferedItems();

        // Init new barter
        ResetData();

        // Load NPC Data
        NPCOfferSlotOne.SetData(_currentTradeInformation.ItemOnOffer, false);
        NPCValueText.text = "Value: " + _currentTradeInformation.ItemOnOffer.BaseValue;
        NPCProfilePicture.sprite = _currentTradeInformation.NPCData.Icon;

        // Activate Pre-Barter Effect Cards
        ActivateEffectCards(true);
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Offer a item to the barter pool.
    /// Called by UI Elements.
    /// </summary>
    /// <param name="itemToOffer">Item to offer to the offer pool.</param>
    public void OfferItem(InventoryCardData itemToOffer) {

        if (itemToOffer == null) return;

        if (_offeredItems.Count >= 4) return;

        _offeredItems.Add(itemToOffer);

        UpdateVisuals();
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
    /// End barter and determines if player pool is valuable enough for NPC.
    /// Called by UI Elements.
    /// </summary>
    public void EndBarter() {

        ActivateEffectCards(false);

        float NPCItemValue = _currentTradeInformation.ItemOnOffer.BaseValue;

        EndMessageSpeechBubble.SetActive(true);

        if (_currentOfferedValue >= NPCItemValue) {
            // Complete Trade
            EndMessage.text = _currentTradeInformation.DialogueForTrade;
            PassBarterIcon.SetActive(true);
            _wonBarter = true;
        } else {
            // Say no!
            EndMessage.text = _currentTradeInformation.DialogueForNoTrade;
            FailBarterIcon.SetActive(true);
        }

        StartCoroutine(LeaveBarterScene());
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Activates Effect Cards
    /// </summary>
    /// <param name="isPreBarter">which stage of the barter are we in for activating the effect cards?</param>
    private void ActivateEffectCards(bool isPreBarter)
    {
        // TODO: Activate the Effect Cards
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
            PlayerOfferSlotOne.SetData(_offeredItems.Items[0]);
        }
        if (_offeredItems.Count >= 2) {
            PlayerOfferSlotTwo.SetData(_offeredItems.Items[1]);
        }
        if (_offeredItems.Count >= 3)
        {
            PlayerOfferSlotThree.SetData(_offeredItems.Items[2]);
        }
        if (_offeredItems.Count >= 4)
        {
            PlayerOfferSlotFour.SetData(_offeredItems.Items[3]);
        }

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
        PlayerOfferSlotOne.SetCardToEmpty(true);
        PlayerOfferSlotTwo.SetCardToEmpty(true);
        PlayerOfferSlotThree.SetCardToEmpty(true);
        PlayerOfferSlotFour.SetCardToEmpty(true);
        _currentOfferedValue = 0;
        PlayerValueText.text = "Value: 0";
    }

    private void ResetNPCData() {
        NPCOfferSlotOne.SetCardToEmpty(true);
        NPCValueText.text = "Value: 0";
    }

    IEnumerator LeaveBarterScene() {
        yield return new WaitForSeconds(1f);

        _offeredItems.ReturnCardsToInventory();

        if (_wonBarter) {
            // remove cards offered
            foreach (InventoryCardData card in _offeredItems.Items)
            {
                GameManager.Inventory.RemoveCard(card);
            }

            GameManager.Inventory.AddCard(_currentTradeInformation.ItemOnOffer);
        }

        _offeredItems = null;

        GameManager.Inventory.ResetAllCardValues();

        InGameUi _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();

        _inGameUi.MoveToDefault();
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