using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUiNavigation : MonoBehaviour {
    [SerializeField] private SettingsUi SettingsUi;
    [SerializeField] private InGameUi InGameUi;

    public void Update() {

        if (GameManager.PlayerInput.GetMenu1Down() && !InGameUi.InBartering() || GameManager.PlayerInput.GetStartDown()) {
            // NOTE: REPLACED INVENTORY CODE WITH PAUSE
            if (GameManager.CurrentUIManager == SettingsUi) {
                InGameUi.SwapToInGameUi();
            } else {
                SettingsUi.SwapToSettingsUi();
            }
        }
    }

    #if !UNITY_EDITOR
    void OnApplicationFocus(bool focus)
    {
        if (GameManager.CurrentUIManager != SettingsUi) {
            SettingsUi.SwapToSettingsUi();
        }
    }
    #endif
}
