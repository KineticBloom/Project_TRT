using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class StartUi : UiManager<StartUi.UiStates> {

    public enum UiStates {
        Title,
        Credits,
        Options,
        AccessibilityCheck,
        Controls,
        CheckToQuit,
        CheckToNewGame
    }

    private void Start() {
        GameManager.Instance.SwapUiManager(this);
    }

    public void MoveToTitle() => MoveTo(UiStates.Title);
    public void MoveToCredits() => MoveTo(UiStates.Credits);
    public void MoveToOptions() => MoveTo(UiStates.Options);
    public void MoveToAccessibilityCheck() => MoveTo(UiStates.AccessibilityCheck);
    public void MoveToNewGame() {
        SaveSystem.ResetSaveData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void MoveToContinueGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void MoveToQuit() {
        Application.Quit();
    }
    public void MoveToControls() => MoveTo(UiStates.Controls);

    public void MoveToCheckToQuit() {
        MoveTo(UiStates.CheckToQuit);
    }

    public void MoveToCheckToNewGame() {
        MoveTo(UiStates.CheckToNewGame);
    }

}
