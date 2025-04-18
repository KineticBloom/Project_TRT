using UnityEngine;
using UnityEngine.SceneManagement;

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
        Controls
    }

    public void MoveToPause() => MoveTo(UiStates.Pause);
    public void MoveToOptions() => MoveTo(UiStates.Options);
    public void MoveToControls() => MoveTo(UiStates.Controls);

    public void MoveToTitle() {
        SceneManager.LoadScene(0);
    }

    public void SwapToSettingsUi() {
        GameManager.Instance.SwapUiManager(this);
        pauseOpen.Post(this.gameObject);
    }

    protected override void DisableFocus() {
        base.DisableFocus();
        pauseClose.Post(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
