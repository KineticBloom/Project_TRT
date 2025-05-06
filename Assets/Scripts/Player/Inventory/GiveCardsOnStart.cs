using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCardsOnStart : MonoBehaviour
{
    [SerializeField] private List<InventoryCardData> cards;
    // Start is called before the first frame update
    void Start()
    {
        foreach (InventoryCardData card in cards)
        {
            GameManager.Inventory.AddCard(card, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
