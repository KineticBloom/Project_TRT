using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveCardsOnStart : MonoBehaviour
{
    [SerializeField] private List<InventoryCardData> cards;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AddSoon());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AddSoon()
    {
        yield return new WaitForSecondsRealtime(0.02f);
        if (GameManager.Instance != null)
        {
            foreach (InventoryCardData card in cards)
            {
                GameManager.Inventory.AddCard(card, true);
            }
        }
    }
}
