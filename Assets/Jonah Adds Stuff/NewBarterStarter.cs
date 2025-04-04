using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBarterStarter : MonoBehaviour
{
    private InGameUi _inGameUi;

    public void Start() {
        if (GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }
    }

    public void StartBarter(BarteringController.TradeData tradeData) {

        if (_inGameUi == null) return;

        _inGameUi.MoveToBartering(tradeData);
    }

    public BarteringController GetBarteringController() {

        if (_inGameUi == null) return null;

        return _inGameUi.BarteringController;
    }
}
