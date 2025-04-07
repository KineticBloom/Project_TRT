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

        if (_offeredItems.Count >= 2) return;

        _offeredItems.Add(GameManager.Inventory.GetCardFromData(itemToOffer));

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

        // Remove item
        _offeredItems.Remove(GameManager.Inventory.GetCardFromData(itemToRemove));

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

    private void UpdateVisuals() {

        ResetPlayerData();

        // Get new player offer value
        foreach (InventoryCard item in _offeredItems.Items) {
            _currentOfferedValue += item.CurrentValue;
        }

        PlayerValueText.text = "Value: " + _currentOfferedValue;

        // Display new slots adjusted
        if (_offeredItems.Count >= 1) {
            PlayerOfferSlotOne.SetData(_offeredItems.Items[0].Data);
        }
        if (_offeredItems.Count >= 2) {
            PlayerOfferSlotTwo.SetData(_offeredItems.Items[1].Data);
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
            foreach (InventoryCard card in _offeredItems.Items)
            {
                GameManager.Inventory.RemoveCard(card.Data);
            }

            GameManager.Inventory.AddCard(_currentTradeInformation.ItemOnOffer);
        }

        _offeredItems = null;

        InGameUi _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();

        _inGameUi.MoveToDefault();
    }

    #endregion
}

[System.Serializable]
public class OfferedItems
{
    public List<InventoryCard> Items;
    public int Count {  get { return Items.Count; } }


    public OfferedItems()
    {
        Items = new List<InventoryCard>();
    }

    public void Add(InventoryCard card)
    {
        Items.Add(card);
        GameManager.Inventory.RemoveCard(card.Data, true);
    }

    public void Remove(InventoryCard card)
    {
        Items.Remove(card);
        GameManager.Inventory.AddCard(card.Data, true);
    }

    public void ReturnCardsToInventory()
    {
        foreach (InventoryCard card in Items)
        {
            GameManager.Inventory.AddCard(card.Data, true);
        }
    }
}