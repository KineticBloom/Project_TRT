using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;


[CreateAssetMenu(fileName = "AllInventoryCardDatas", menuName = "ScriptableObjects/AllICDatas", order = 1)]
public class AllInventoryCardDatas : ScriptableObject
{
    public List<InventoryCardData> datas = new();
}