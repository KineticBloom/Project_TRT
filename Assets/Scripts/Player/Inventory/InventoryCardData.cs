using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using static GameEnums;


[CreateAssetMenu(fileName = "InventoryCard", menuName = "ScriptableObjects/InventoryCard", order = 1)]
public class InventoryCardData : ScriptableObject
{
    public string CardName;

    public string ID;
    public string Description;
    public Sprite Sprite;
    public int BaseValue;

    [SerializeField, ReadOnly]
    private int _currentValue = 0;

    public int CurrentValue => _currentValue;
    public void SetCurrentValue(int value) => _currentValue = value;

    private void OnEnable()
    {
        _currentValue = BaseValue;
    }
}