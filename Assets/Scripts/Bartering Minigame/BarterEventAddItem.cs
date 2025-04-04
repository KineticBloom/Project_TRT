/// <summary>
/// Event to offer a selected item in the barter.
/// InventoryAction: Used when item in inventory grid is selected.
/// </summary>
public class BarterEventAddItem : InventoryAction
{
    /// <summary>
    /// Offer selected item to the barterController.
    /// </summary>
    /// <param name="context">Contains information on the card selected.</param>
    public override void ActionOnClick(ActionContext context) {
        BarteringController controller = GameManager.NewBarterStarter.GetBarteringController();

        controller.OfferItem(context.cardData);
    }

}
