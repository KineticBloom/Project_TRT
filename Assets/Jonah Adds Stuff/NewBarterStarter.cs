using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBarterStarter : MonoBehaviour
{
    public InGameUi GameUIController;
    public BarteringController BarterController;

    BarteringController.TradeData tradeData;

    public void StartBarter(BarteringController.TradeData tradeData) {

        GameUIController.MoveToBartering();

        this.tradeData = tradeData;

        StartCoroutine("SetupTrade");
    }

    public BarteringController GetBarteringController() {
        return BarterController;
    }

    IEnumerator SetupTrade() {
        yield return 0;

        // Setup Barter Director
        BarterController.InitializeTrade(tradeData);
    }
}
