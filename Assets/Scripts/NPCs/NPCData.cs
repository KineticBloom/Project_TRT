using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using MackySoft.SerializeReferenceExtensions;

[CreateAssetMenu(fileName = "New NPCData", menuName = "ScriptableObjects/NPCData"), System.Serializable]
public class NPCData : ScriptableObject
{
    #region ======== [ VARIABLES ] ========

    [Header("Profile Details")]
    public string Name;
    public Sprite Icon;
    [TextArea] public string Bio;

    [Header("Bartering Details")]
    [Tooltip("Defines the Effect Cards that will affect the bartering game")]
    public List<EffectCardData> EffectCards = new List<EffectCardData>();
    [Tooltip("How many attempts the player can have to barter with the NPC\n\n" +
        "A negative value is unlimited attempts")] 
    public int BarterAttempts = -1;



    #endregion
}
