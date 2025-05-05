using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AllNPCDatas", menuName = "ScriptableObjects/AllNPCDatas", order = 1)]
public class AllNPCDatas : ScriptableObject
{
    public List<NPCData> datas = new();
}