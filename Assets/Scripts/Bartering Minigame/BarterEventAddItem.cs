using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarterEventAddItem : InventoryAction
{

    public override void ActionOnClick(ActionContext context) {
        BarteringController controller = GameManager.NewBarterStarter.GetBarteringController();

        controller.AddItem(context.cardData);
    }

}
