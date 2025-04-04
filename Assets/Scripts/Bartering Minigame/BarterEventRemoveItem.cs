using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clears a item from a given InventoryCardObject.
/// InventoryCardObject's specifically used to display an item offered in the barter.
/// </summary>
public class BarterEventRemoveItem : MonoBehaviour {

    [Tooltip("Container that will have their item removed.")]
    public InventoryCardObject card;

    /// <summary>
    /// Remove a item from the barterController.
    /// </summary>
    public void PopItemFromController() {
        BarteringController controller = GameManager.NewBarterStarter.GetBarteringController();

        controller.RetractItem(card.Card);
    }
}
