using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

[CreateAssetMenu(fileName = "New NPCData", menuName = "ScriptableObjects/NPCData"), System.Serializable]
public class NPCData : ScriptableObject
{
    #region ======== [ VARIABLES ] ========

    public string Name;
    public Sprite Icon;
    [TextArea] public string Bio;

    #endregion
}
