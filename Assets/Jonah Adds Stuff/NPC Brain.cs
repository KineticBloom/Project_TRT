using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBrain : Interactable {

    public NPCData DataOfThisNPC;
    public InventoryCardData ItemForOffer;
    public string DialogueForTrade;
    public string DialogueForNoTrade;

    public override void Interaction() {

        BarteringController.TradeData tradeData = new BarteringController.TradeData();
        tradeData = new BarteringController.TradeData();
        tradeData.ItemOnOffer = ItemForOffer;
        tradeData.NPCData = DataOfThisNPC;
        tradeData.DialogueForTrade = DialogueForTrade;
        tradeData.DialogueForNoTrade = DialogueForNoTrade;

        GameManager.NewBarterStarter.StartBarter(tradeData);
    }

    public override void Highlight() {
        // TODO: Add Highlight Shader
    }
    public override void UnHighlight() {
        // TODO: Add Highlight Shader
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + IconLocalPosition, 0.25f);
    }
}
