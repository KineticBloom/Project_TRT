using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using static InventoryCardObject;
using DG.Tweening;

/// <summary>
/// Tracks Setting UI panels
/// </summary>
public class SettingsUi : UiManager<SettingsUi.UiStates> {

    [Header("Animation")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private float tweenDuration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    [Header("Audio")]
    [SerializeField] private AK.Wwise.Event pauseOpen;
    [SerializeField] private AK.Wwise.Event pauseClose;

    private Vector2 offscreenPos;
    private float centerPos;

    public enum UiStates {
        Pause,
        Options,
        Controls,
        CheckToTitle
    }

    private void Awake()
    {
        centerPos = 1000;
        offscreenPos = new Vector2(GetComponentInParent<RectTransform>().rect.width*1.5f, GetComponentInParent<RectTransform>().rect.height/2);
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

        // Animation in
        settingsPanel.transform.position = new Vector3(offscreenPos.x, offscreenPos.y);
        settingsPanel
            .transform
            .DOMoveX(centerPos, tweenDuration)
            .SetEase(ease);
    }

    protected override void DisableFocus() {
        // Animation out
        settingsPanel
            .transform
            .DOMoveX(offscreenPos.x, tweenDuration)
            .SetEase(ease)
            .OnComplete(() => AfterLeave());
    }

    private void AfterLeave()
    {
        this._currentStateData.StatesCanvasGroup.gameObject.SetActive(false);
    }
}
