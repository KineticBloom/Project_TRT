using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameEnums;
using System.Linq;
using System.Xml.Linq;

public class JournalNPC : InventoryAction
{
    #region ======== [ VARIABLES ] ========

    [Header("NPC Info")]
    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private Image iconDisplay;
    [SerializeField] private TextMeshProUGUI bioDisplay;

    [Header("Known Trades")]
    [SerializeField] private Image playerItem;
    [SerializeField] private Image oppItem;
    [SerializeField] private Sprite oppItemNoTradeKnown;

    private NPC _npcData;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Adds this NPCData to the known NPCs for the NPC. Will do nothing if the NPC is already known.
    /// </summary>
    /// <param name="npcData"></param>
    public void AddNPC(NPCData npcData)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("Cannot AddNPC, GameManager.Instance is null");
            return;
        }

        GameManager.NPCGlobalList.AddKnownNPC(GameManager.NPCGlobalList.GetNPCFromData(npcData));

    }


    public bool IsKnown(NPCData npcData)
    {
        NPC wrapper = GameManager.NPCGlobalList.GetNPCFromData(npcData);

        return GameManager.NPCGlobalList.KnownNPCs.Contains(wrapper);
    }


    /// <summary>
    /// Loads this NPC to be displayed in the journal.
    /// </summary>
    /// <param name="npc"></param>
    public void LoadNPC(NPCData npc)
    {
        _npcData = GameManager.NPCGlobalList.GetNPCFromData(npc);

        nameDisplay.text = npc.Name;
        iconDisplay.sprite = npc.Icon;
        bioDisplay.text = npc.Bio;
    }


    /// <summary>
    /// Displays the trade for the selected item
    /// </summary>
    /// <param name="context"></param>
    public override void ActionOnClick(ActionContext context)
    {
        InventoryCardData cardData = context.cardData;

        playerItem.sprite = cardData ? cardData.Sprite : null;
        oppItem.sprite = _npcData.GetKnownTrade(cardData) ? 
            _npcData.GetKnownTrade(cardData).Sprite : oppItemNoTradeKnown;

        if(playerItem.sprite == null) {
            playerItem.color = Color.clear;
            oppItem.color = Color.clear;
        } else {
            playerItem.color = Color.white;
            oppItem.color = Color.white;
        }
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    
   

    #endregion
}