using System.Collections;
using UnityEngine;
/// <summary>
/// Tracks Gameplay UI panels
/// </summary>
public class InGameUi : UiManager<InGameUi.UiStates> {

    public BarteringController BarteringController;
    public DialogueUiManager DialogueUiManager;
    public NotificationUI Notification;

    private void Start() {
        GameManager.Instance.SwapUiManager(this);
    }

    public new enum UiStates {
        Default,
        Bartering,
        Dialogue
    }

    public void MoveToDefault() => MoveTo(UiStates.Default);
    
    public void MoveToBartering(NPCData npcData) {
        BarteringController.InitializeTrade(npcData);
        MoveTo(UiStates.Bartering);
    }
    public void MoveToDialogue() => MoveTo(UiStates.Dialogue);

    public void SwapToInGameUi() {
        StartCoroutine(LoadInGameUI());
    }

    IEnumerator LoadInGameUI() {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.SwapUiManager(this);
        LoadState(_currentState);
    }
}

