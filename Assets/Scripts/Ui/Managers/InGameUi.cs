
/// <summary>
/// Tracks Gameplay UI panels
/// </summary>
public class InGameUi : UiManager<InGameUi.UiStates> {

    public BarteringController BarteringController;
    public DialogueUiManager DialogueUiManager;
    public NotificationUI Notification;

    public new enum UiStates {
        Default,
        Bartering,
        Dialogue
    }

    public void MoveToDefault() => MoveTo(UiStates.Default);
    public void MoveToBartering(BarteringController.TradeData tradeData) {
        BarteringController.InitializeTrade(tradeData);
        MoveTo(UiStates.Bartering);
    }
    public void MoveToDialogue() => MoveTo(UiStates.Dialogue);

    public void SwapToInGameUi() {
        GameManager.Instance.SwapUiManager(this);
    }
}


