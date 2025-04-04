using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBarterStarter : MonoBehaviour
{
    private InGameUi _inGameUi;

    public void Start() {
        if (GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoadTrigger;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoadTrigger;
    }

    public void StartBarter(BarteringController.TradeData tradeData) {

        if (_inGameUi == null) return;

        _inGameUi.MoveToBartering(tradeData);
    }

    public BarteringController GetBarteringController() {

        if (_inGameUi == null) return null;

        return _inGameUi.BarteringController;
    }

    void OnSceneLoadTrigger(Scene scene, LoadSceneMode mode) {

        StartCoroutine("OnSceneLoad");
    }

    IEnumerator OnSceneLoad() {
        yield return 0;
        if (_inGameUi == null && GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }
    }
}
