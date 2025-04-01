using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryCardBarterPop : MonoBehaviour {

    public InventoryCardObject card;

    public void PopItemFromController() {
        BarteringController controller = GameManager.NewBarterStarter.GetBarteringController();

        controller.RemoveItem(card.Card);
    }
}
