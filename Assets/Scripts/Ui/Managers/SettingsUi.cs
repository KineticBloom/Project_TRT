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
    [SerializeField] private InventoryBar inventoryBar;
    [SerializeField] private float tweenDuration = 0.5f;
    [SerializeField] private float smallScale = 0.75f;
    [SerializeField] private Ease enterEase = Ease.OutCubic;
    [SerializeField] private Ease exitEase = Ease.InCubic;
    [SerializeField] private float growDelay = 0.2f;
    [SerializeField] private float shrinkDelay = 0.1f;
    [SerializeField] private Ease growEase = Ease.InOutExpo;
    [SerializeField] private Ease shrinkEase = Ease.InOutExpo;

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
        MoveToPause();
        pauseOpen.Post(this.gameObject);
        LoadSettingsUi();
    }


    private void LoadSettingsUi()
    {
        inventoryBar.SetActiveSource(gameObject, true);

        GameManager.Instance.SwapUiManager(this);
        LoadState(_currentState);

        // Animation in
        settingsPanel.localPosition = Vector3.right * settingsPanel.rect.width;
        settingsPanel.localScale = Vector3.one * smallScale;

        settingsPanel.DOKill();
        settingsPanel.transform.DOLocalMoveX(0, tweenDuration, true)
            .SetEase(enterEase).SetUpdate(true);
        settingsPanel.transform.DOScale(1f, tweenDuration)
            .SetEase(growEase).SetUpdate(true).SetDelay(growDelay); 
    }


    protected override void DisableFocus() 
    {
        base.DisableFocus();
        inventoryBar.SetActiveSource(gameObject, false);

        if (!settingsPanel.gameObject.activeSelf)
        {
            AfterLeave();
            return;
        }

        // Animation out
        settingsPanel.DOKill();
        settingsPanel.transform.DOLocalMoveX(settingsPanel.rect.width, tweenDuration)
            .SetEase(exitEase).SetUpdate(true)
            .OnComplete(() => AfterLeave());
        settingsPanel.transform.DOScale(smallScale, tweenDuration)
            .SetEase(shrinkEase).SetUpdate(true).SetDelay(shrinkDelay);
    }


    private void AfterLeave()
    {
        this._currentStateData.StatesCanvasGroup.gameObject.SetActive(false);
    }
}
