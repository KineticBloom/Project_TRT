using System.Collections;
using UnityEngine;
/// <summary>
/// Tracks Gameplay UI panels
/// </summary>
public class InGameUi : UiManager<InGameUi.UiStates> {

    public BarteringController BarteringController;
    public ChooseItemCanvasController ChooseItemCanvasController;
    public DialogueUiManager DialogueUiManager;
    public NotificationUI Notification;
    public InventoryBar InventoryBar;

    private void Start() {
        GameManager.Instance.SwapUiManager(this);
    }

    public new enum UiStates {
        Default,
        Bartering,
        Dialogue,
        BarteringChooseItem
    }

    public void SetInventoryBarInteractable(bool interactable, bool isEnabled)
    {
        InventoryBar.gameObject.SetActive(isEnabled);
        InventoryBar.SetInteractable(interactable);
    }

    public void MoveToDefault() => MoveTo(UiStates.Default);
    
    public void MoveToBartering(NPCData npcData, InventoryCardData cardOnOffer, NpcInteractable npcInstance) {
        BarteringController.InitializeTrade(npcData, cardOnOffer, npcInstance);
        MoveTo(UiStates.Bartering);
    }
    public void MoveToDialogue() => MoveTo(UiStates.Dialogue);

    public void MoveToChooseItem(NPCData npcData, NpcInteractable npcInstance) {
        ChooseItemCanvasController.InitOffer(npcData, npcInstance);
        MoveTo(UiStates.BarteringChooseItem);
    }

    public void SwapToInGameUi() {
        StartCoroutine(LoadInGameUI());
    }

    IEnumerator LoadInGameUI() {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.SwapUiManager(this);
        LoadState(_currentState);
    }

    public override void GoBack() {return;}
}

