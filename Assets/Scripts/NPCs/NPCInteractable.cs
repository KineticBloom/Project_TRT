using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcInteractable : Interactable
{
    [SerializeField] private TextAsset npcConversation;
    [Expandable] public NPCData NpcData;

    public AudioEvent interactionBarkSFX;
    public AudioEvent barterBarkSFX;

    public Vector3 DialogueSourceLocalPosition;
    public override void Interaction() {
        Vector3 NPCWorldPosition = this.transform.position + DialogueSourceLocalPosition;
        Vector3 PlayerWorldPosition = GameManager.Player.DialogueSource.position;
        GameManager.DialogueManager.StartDialogue(npcConversation, TriggerBarter, NPCWorldPosition, PlayerWorldPosition);

        interactionBarkSFX.Play(gameObject);
    }

    public void TriggerBarter() {

        barterBarkSFX.Play(gameObject);
        GameManager.NewBarterStarter.StartBarter(NpcData);
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

    /// <summary>
    /// Loads Effect Cards from save data
    /// </summary>
    /// <param name="data"></param>
    public void Load(NPCSaveData data)
    {
        if (data.effectCardData == null)
        {
            Debug.LogError("NPCInteractable: Cannot Load null data");
            return;
        }

        // Cannot load data that does not exist
        if (!data.effectCardData.ContainsKey(NpcData.FlagID))
        {
            return;
        }

        List<bool> effectCardsList = data.effectCardData[NpcData.FlagID];

        // Set reveal status for all Effect Cards
        for (int effectCardIndex = 0; effectCardIndex < NpcData.EffectCards.Count; effectCardIndex++)
        {
            NpcData.EffectCards[effectCardIndex].IsRevealed = effectCardsList[effectCardIndex];
        }
    }
}