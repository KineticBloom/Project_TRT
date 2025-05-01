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
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private float tweenDuration = 0.5f;
    [SerializeField] private float smallScale = 0.75f;
    [SerializeField] private Ease enterEase = Ease.OutCubic;
    [SerializeField] private Ease exitEase = Ease.InCubic;
    [SerializeField] private Ease sizeEase = Ease.InOutCubic;

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

    public void SwapToSettingsUi() 
    {
        pauseOpen.Post(this.gameObject);
        LoadSettingsUi();
    }


    private void LoadSettingsUi() 
    {
        Debug.Log("LoadSettingsUi");
        GameManager.Instance.SwapUiManager(this);
        LoadState(_currentState);

        // Animation in
        settingsPanel.localPosition = Vector3.right * settingsPanel.rect.width;
        settingsPanel.localScale = Vector3.one * smallScale;

        settingsPanel.DOKill();
        settingsPanel.transform.DOLocalMoveX(0, tweenDuration, true)
            .SetEase(enterEase).SetUpdate(true);
        settingsPanel.transform.DOScale(1f, tweenDuration)
            .SetEase(sizeEase).SetUpdate(true);
    }


    protected override void DisableFocus() 
    {
        // Animation out
        settingsPanel.DOKill();
        settingsPanel.transform.DOLocalMoveX(settingsPanel.rect.width, tweenDuration)
            .SetEase(exitEase).SetUpdate(true)
            .OnComplete(() => AfterLeave());
        settingsPanel.transform.DOScale(smallScale, tweenDuration)
            .SetEase(sizeEase).SetUpdate(true);
    }


    private void AfterLeave()
    {
        this._currentStateData.StatesCanvasGroup.gameObject.SetActive(false);
    }
}
