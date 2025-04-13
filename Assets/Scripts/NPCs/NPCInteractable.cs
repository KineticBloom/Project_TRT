using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class NpcInteractable : Interactable
{
    [SerializeField] private TextAsset npcConversation;

    public NPCData NpcData;
    public InventoryCardData ItemForOffer;
    public string BarterMessageWin;
    public string BarterMessageLose;
    public override void Interaction() {

        BarteringController.TradeData tradeData = new BarteringController.TradeData();

        tradeData = new BarteringController.TradeData();
        tradeData.ItemOnOffer = ItemForOffer;
        tradeData.NPCData = NpcData;
        tradeData.DialogueForTrade = BarterMessageWin;
        tradeData.DialogueForNoTrade = BarterMessageLose;

        GameManager.NewBarterStarter.StartBarter(tradeData);
    }

    public override void Highlight()
    {
        // TODO: Add Highlight Shader
    }

    public override void UnHighlight()
    {
        // TODO: Remove Highlight Shader
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + IconLocalPosition, 0.25f);
    }
}
