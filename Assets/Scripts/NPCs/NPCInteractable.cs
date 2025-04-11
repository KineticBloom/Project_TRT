using Ink.UnityIntegration;
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

    public Vector3 DialogueSourceLocalPosition;
    public override void Interaction() {
        Vector3 NPCWorldPosition = this.transform.position + DialogueSourceLocalPosition;
        Vector3 PlayerWorldPosition = GameManager.Player.DialogueSource.position;
        GameManager.DialogueManager.StartDialogue(npcConversation, TriggerBarter, NPCWorldPosition, PlayerWorldPosition);
    }

    public void TriggerBarter() {
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + IconLocalPosition, 0.25f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + DialogueSourceLocalPosition, Vector3.one * 0.25f );
    }
}
