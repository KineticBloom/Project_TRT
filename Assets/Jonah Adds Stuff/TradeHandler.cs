using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles opening and running the trade screen!
/// </summary>
public class TradeHandler : MonoBehaviour {
    public struct TradeData {
        public InventoryCardData ItemOnOffer;
        public string DialogueForTrade;
        public string DialogueForNoTrade;
    }

    public InGameUi CanvasManager;
    public Image NPCTradeSlot;

    public void StartTrade(TradeData TradeInformation) {

        // Switch UI States
        CanvasManager.MoveToBartering();

        // Setup Trade
        NPCTradeSlot.sprite = TradeInformation.ItemOnOffer.Sprite;

    }


}
