using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    public GameObject FAILBARTERICON;
    public GameObject PASSBARTERICON;
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

    private int _limitOfItems = 2;
    private TradeData _currentTradeInformation;
    private float _currentOfferedValue;
    private bool _wonBarter = false;


    List<InventoryCardData> _items = new List<InventoryCardData>();

    #endregion

    #region ======== [ INIT METHOD ] ========

    public void InitializeTrade(TradeData TradeInformation) {

        // Reset information

        PlayerOfferSlotOne.SetCardToEmpty(true);
        PlayerOfferSlotTwo.SetCardToEmpty(true);
        NPCOfferSlotOne.SetCardToEmpty(true);
        PlayerValueText.text = "Value: 0";
        FAILBARTERICON.SetActive(false);
        PASSBARTERICON.SetActive(false);
        EndMessageSpeechBubble.SetActive(false);
        _items.Clear();
        _wonBarter = false;

        Debug.Log("Start barter " + TradeInformation.ItemOnOffer.name);

        _currentTradeInformation = TradeInformation;

        NPCOfferSlotOne.SetData(_currentTradeInformation.ItemOnOffer, false);
        NPCValueText.text = "Value: " + _currentTradeInformation.ItemOnOffer.ValueOfItem;
        NPCProfilePicture.sprite = _currentTradeInformation.NPCData.Icon;
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========
    public void AddItem(InventoryCardData itemToAdd) {

        if (itemToAdd == null) return;

        if (_items.Count >= _limitOfItems) {
            return;
        }
        _items.Add(itemToAdd);
        UpdateVisuals();
    }

    public void RemoveItem(InventoryCardData itemToRemove) {
        _items.Remove(itemToRemove);

        PlayerOfferSlotOne.SetCardToEmpty(true);
        PlayerOfferSlotTwo.SetCardToEmpty(true);

        if (_items.Count == 1) {
            PlayerOfferSlotTwo.CurrentActiveButton.Select();
        }
        if (_items.Count == 0) {
            PlayerOfferSlotOne.CurrentActiveButton.Select();
        }

        UpdateVisuals();
    }

    public void ClearItems() {
        _items.Clear();
        PlayerOfferSlotOne.SetCardToEmpty(true);
        PlayerOfferSlotTwo.SetCardToEmpty(true);
        UpdateVisuals();
    }

    public void ProcessBarter() {

        float NPCItemValue = _currentTradeInformation.ItemOnOffer.ValueOfItem;

        EndMessageSpeechBubble.SetActive(true);

        if (_currentOfferedValue >= NPCItemValue) {
            // Complete Trade
            EndMessage.text = _currentTradeInformation.DialogueForTrade;
            PASSBARTERICON.SetActive(true);
            _wonBarter = true;
        } else {
            // Say no!
            EndMessage.text = _currentTradeInformation.DialogueForNoTrade;
            FAILBARTERICON.SetActive(true);
        }

        StartCoroutine("ExitBarter");
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    private void UpdateVisuals() {

        float value = 0;
        int count = 0;

        foreach (InventoryCardData item in _items) {
            if (item != null) {
                value += item.ValueOfItem;
                count += 1;
            }
            if (count >= _limitOfItems) {
                break;
            }
        }

        _currentOfferedValue = value;
        PlayerValueText.text = "Value: " + value;

        if (_items.Count >= 1) {
            PlayerOfferSlotOne.SetData(_items[0]);
        }
        if (_items.Count >= 2) {
            PlayerOfferSlotTwo.SetData(_items[1]);
        }

    }

    IEnumerator ExitBarter() {
        yield return new WaitForSeconds(1f);

        GameManager.NewBarterStarter.GameUIController.MoveToDefault();

        if (_wonBarter) {
            GameManager.Inventory.AddCard(_currentTradeInformation.ItemOnOffer);
        }

    }

    #endregion
}
