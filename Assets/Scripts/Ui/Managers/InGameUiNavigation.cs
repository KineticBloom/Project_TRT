using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUiNavigation : MonoBehaviour {
    [SerializeField] private SettingsUi SettingsUi;
    [SerializeField] private InGameUi InGameUi;


    public void Update() {

        if (GameManager.PlayerInput.GetMenu1Down() || GameManager.PlayerInput.GetStartDown()) {
            // NOTE: REPLACED INVENTORY CODE WITH PAUSE
            if (GameManager.CurrentUIManager == SettingsUi) {
                GameManager.Instance.SwapUiManager(InGameUi);
            } else {
                GameManager.Instance.SwapUiManager(SettingsUi);
            }
        }
    }
}
