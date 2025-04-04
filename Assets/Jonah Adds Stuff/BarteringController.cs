using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private List<InventoryCardData> _offeredItems;

    #endregion

    #region ======== [ INIT METHOD ] ========
    public void InitializeTrade(TradeData TradeInformation) {

        // Setup trackers
        _currentTradeInformation = TradeInformation;
        _offeredItems = new List<InventoryCardData>();

        // Init new barter
        ResetData();

        // Load NPC Data
        NPCOfferSlotOne.SetData(_currentTradeInformation.ItemOnOffer, false);
        NPCValueText.text = "Value: " + _currentTradeInformation.ItemOnOffer.ValueOfItem;
        NPCProfilePicture.sprite = _currentTradeInformation.NPCData.Icon;
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========
    public void AddItem(InventoryCardData itemToAdd) {

        if (itemToAdd == null) return;

        if (_offeredItems.Count >= 2) return;

        _offeredItems.Add(itemToAdd);

        UpdateVisuals();
    }

    public void RemoveItem(InventoryCardData itemToRemove) {

        // See what button was activated
        _currentButtonObject = null;

        if (PlayerOfferSlotOne.CurrentActiveButton.gameObject == EventSystem.current.currentSelectedGameObject) {
            _currentButtonObject = PlayerOfferSlotOne;
        }
        if (PlayerOfferSlotTwo.CurrentActiveButton.gameObject == EventSystem.current.currentSelectedGameObject) {
            _currentButtonObject = PlayerOfferSlotTwo;
        }

        // Remove item
        _offeredItems.Remove(itemToRemove);

        UpdateVisuals();

        // Reset selection of button!
        if (_currentButtonObject != null) {
            _currentButtonObject.CurrentActiveButton.Select();
        }
    }

    public void ProcessBarter() {

        float NPCItemValue = _currentTradeInformation.ItemOnOffer.ValueOfItem;

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

        StartCoroutine("ExitBarter");
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    private void UpdateVisuals() {

        ResetPlayerData();

        // Get new player offer value
        foreach (InventoryCardData item in _offeredItems) {
            _currentOfferedValue += item.ValueOfItem;
        }

        PlayerValueText.text = "Value: " + _currentOfferedValue;

        // Display new slots adjusted
        if (_offeredItems.Count >= 1) {
            PlayerOfferSlotOne.SetData(_offeredItems[0]);
        }
        if (_offeredItems.Count >= 2) {
            PlayerOfferSlotTwo.SetData(_offeredItems[1]);
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

    IEnumerator ExitBarter() {
        yield return new WaitForSeconds(1f);

        InGameUi _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();

        _inGameUi.MoveToDefault();

        if (_wonBarter) {
            Debug.Log("Won barter!");
            GameManager.Inventory.AddCard(_currentTradeInformation.ItemOnOffer);
        }

    }

    #endregion
}
