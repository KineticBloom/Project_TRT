using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
#if UNITY_EDITOR
using MackySoft.SerializeReferenceExtensions;   // Don't Remove, this is actually needed
#endif

[CreateAssetMenu(fileName = "New NPCData", menuName = "ScriptableObjects/NPCData"), System.Serializable]
public class NPCData : ScriptableObject
{
    #region ======== [ VARIABLES ] ========

    [Header("Profile Details")]
    public string Name;
    public Sprite Icon;
    [TextArea] public string Bio;

    [Header("Bartering Details")]
    [Tooltip("The Flag ID of the character's barter")]
    public string FlagID;
    [Tooltip("Defines the Effect Cards that will affect the bartering game")]
    [SerializeReference, SubclassSelector]
    public List<EffectCard> EffectCards = new List<EffectCard>();
    [Tooltip("The item that the NPC gives on a successful trade")]
    public InventoryCardData ItemOnOffer;
    [Tooltip("What the NPC says after a successful trade")]
    public string BarterMessageWin;
    [Tooltip("What the NPC says after an unsuccessful trade")]
    public string BarterMessageLose;
    [Tooltip("How many attempts the player can have to barter with the NPC\n\n" +
        "A negative value is unlimited attempts")]
    public int BarterAttempts = -1;


    #endregion
}
