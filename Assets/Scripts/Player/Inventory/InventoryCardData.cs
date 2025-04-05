using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;


[CreateAssetMenu(fileName = "InventoryCard", menuName = "ScriptableObjects/InventoryCard", order = 1)]
public class InventoryCardData : ScriptableObject
{
    public string CardName;

    public CardTypes Type; // Not Used Anymore

    public string ID;
    public string Description;
    public Sprite Sprite;
    public int ValueOfItem;

    public string StartingLocation; // Not Used Anymore

    public List<ContextOriginPair> ContextData = new List<ContextOriginPair>();
}