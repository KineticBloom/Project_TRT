using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using static InventoryCardObject;

/// <summary>
/// Tracks Setting UI panels
/// </summary>
public class SettingsUi : UiManager<SettingsUi.UiStates> {


    [Header("Audio")]
    [SerializeField] private AK.Wwise.Event pauseOpen;
    [SerializeField] private AK.Wwise.Event pauseClose;

    public enum UiStates {
        Pause,
        Options,
        Controls,
        CheckToTitle
    }

    public void MoveToPause() => MoveTo(UiStates.Pause);
    public void MoveToOptions() => MoveTo(UiStates.Options);
    public void MoveToControls() => MoveTo(UiStates.Controls);

    public void MoveToCheckToTitle() => MoveTo(UiStates.CheckToTitle);

    public void MoveToTitle() {
        SceneManager.LoadScene(0);
    }

    public void SwapToSettingsUi() {
        pauseOpen.Post(this.gameObject);
        StartCoroutine(LoadSettingsUi());
    }
    IEnumerator LoadSettingsUi() {
        yield return new WaitForEndOfFrame();
        GameManager.Instance.SwapUiManager(this);
        LoadState(_currentState);
    }

    protected override void DisableFocus() {
        MoveToPause();
        base.DisableFocus();
        pauseClose.Post(this.gameObject);
        this._currentStateData.StatesCanvasGroup.gameObject.SetActive(false);
    }
}
