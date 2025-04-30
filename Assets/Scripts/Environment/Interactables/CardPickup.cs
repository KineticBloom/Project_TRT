using UnityEngine;
using NaughtyAttributes;

public class CardPickup : Interactable 
{
    [SerializeField] [Required] private InventoryCardData CardToGive;

    // Object Pickup SFX
    [SerializeField] public AudioEvent itemPickupSFX;

    public override void Highlight() {
        // ADD SHADER FOR HIGHLIGHT?
    }

    public override void Interaction() {
        bool ableToPickup = GameManager.Inventory.AddCard(CardToGive);
        if (ableToPickup) {
            itemPickupSFX.Play(this.gameObject);
            Destroy(gameObject);
        }
    }

    public override void UnHighlight() {
        // ADD SHADER FOR HIGHLIGHT?
    }
}
