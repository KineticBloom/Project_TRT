using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class NpcInteractable : Interactable
{
    [SerializeField] private TextAsset npcConversation;
    [Expandable] public NPCData NpcData;
    [SerializeField] private ParticleSystem barterWinParticles;

    public AudioEvent dialogueStartSFX;
    public AudioEvent interactionBarkSFX;
    public AudioEvent barterBarkSFX;
    public AudioEvent barterOpenSFX;

    public Vector3 DialogueSourceLocalPosition;

    public List<InventoryCardData> ItemsAvailable;
    private SquetchStarter _squetchStarter;


    private void Start() 
    {
        _squetchStarter = GetComponent<SquetchStarter>();
        ItemsAvailable = new List<InventoryCardData>(NpcData.ItemsOnOffer);
    }

    public override void Interaction() {
        Vector3 NPCWorldPosition = this.transform.position + DialogueSourceLocalPosition;
        Vector3 PlayerWorldPosition = GameManager.Player.DialogueSource.position;
        _squetchStarter.Subscribe();
        GameManager.DialogueManager.StartDialogue(npcConversation, TriggerBarter, NPCWorldPosition, PlayerWorldPosition);

        dialogueStartSFX.Play(gameObject);
        interactionBarkSFX.Play(gameObject);
    }

    public void TriggerBarter() {

        barterBarkSFX.Play(gameObject);
        barterOpenSFX.Play(gameObject);
        GameManager.NewBarterStarter.StartBarter(NpcData, this);
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

    public void PlayBarterWinParticles()
    {
        barterWinParticles.Play();
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