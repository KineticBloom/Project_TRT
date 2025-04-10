using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Works in editor!

public class AccessibilitySettingsHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [BoxGroup("UI Elements"), SerializeField, Tooltip("The slider controlling our screenshake scaling.")]
    Slider screenshakeSlider;
    [BoxGroup("UI Elements"), SerializeField, Tooltip("The label for our screenshake scaling.")]
    TMP_Text screenshakeReadout;
    [BoxGroup("UI Elements"), SerializeField, Tooltip("The toggle controlling our flashing reduction.")]
    Toggle flashingToggle;
    [BoxGroup("UI Elements"), SerializeField, Tooltip("The toggle controlling our SFX Captions.")]
    Toggle SFXCaptionToggle;
    [BoxGroup("UI Elements"), SerializeField, Tooltip("The toggle controlling our One Hand Mode.")]
    Toggle OneHandModeToggle;

    private float screenshakeScaling;
    private bool disableFlashing;
    private bool enableSFXCaptions;
    private bool enableOneHandMode;

    // ==============================================================
    // Initializers/finalizers
    // ==============================================================

    public static void InitFromPrefs()
    {
        if (PlayerPrefs.HasKey("ScreenshakeAmountPref"))
        {
            Settings.ScreenShakeScalar = PlayerPrefs.GetFloat("ScreenshakeAmountPref");
        }
        if (PlayerPrefs.HasKey("DisableFlashingPref"))
        {
            // If DisableFlashingPref is 1, then disable flashing is true.
            Settings.FlashingDisabled = PlayerPrefs.GetInt("DisableFlashingPref") == 1;
        }
        if (PlayerPrefs.HasKey("SFXCaptionsPref"))
        {
            // If SFXCaptionsPref is 1, then SFX Captions is true.
            Settings.SFXCaptionsEnabled = PlayerPrefs.GetInt("SFXCaptionsPref") == 1;
        }
        if (PlayerPrefs.HasKey("OneHandModePref"))
        {
            // If OneHandModePref is 1, then One Hand Mode is true.
            Settings.OneHandMode = PlayerPrefs.GetInt("OneHandModePref") == 1;
        }
    }

    public void Initialize()
    {
        // Load settings from prefs

        if (PlayerPrefs.HasKey("ScreenshakeAmountPref")) {
            screenshakeScaling = PlayerPrefs.GetFloat("ScreenshakeAmountPref");
            Settings.ScreenShakeScalar = screenshakeScaling;
        } else { // Default value.
            screenshakeScaling = Settings.ScreenShakeScalar;
        }

        if (PlayerPrefs.HasKey("DisableFlashingPref")) {
            // If DisableFlashingPref is 1, then disable flashing is true.
            disableFlashing = PlayerPrefs.GetInt("DisableFlashingPref") == 1;
            Settings.FlashingDisabled = disableFlashing;
        } else { // Default value.
            disableFlashing = Settings.FlashingDisabled;
        }

        if (PlayerPrefs.HasKey("SFXCaptionsPref")) {
            // If SFXCaptionsPref is 1, then SFX Captions is true.
            enableSFXCaptions = PlayerPrefs.GetInt("SFXCaptionsPref") == 1;
            Settings.SFXCaptionsEnabled = enableSFXCaptions;
        } else { // Default value.
            enableSFXCaptions = Settings.SFXCaptionsEnabled;
        }

        if (PlayerPrefs.HasKey("OneHandModePref")) {
            // If OneHandModePref is 1, then One Hand Mode is true.
            enableOneHandMode = PlayerPrefs.GetInt("OneHandModePref") == 1;
            Settings.OneHandMode = enableOneHandMode;
        } else { // Default value.
            enableOneHandMode = Settings.OneHandMode;
        }

        // Load UI values from settings

        if (screenshakeSlider != null) screenshakeSlider.value = screenshakeScaling;
        if (screenshakeReadout != null) UpdateScreenshakeText();

        if (flashingToggle != null) flashingToggle.isOn = disableFlashing;

        if (SFXCaptionToggle != null) SFXCaptionToggle.isOn = enableSFXCaptions;

        if (OneHandModeToggle != null) OneHandModeToggle.isOn = enableOneHandMode;
    }

    private void OnDisable()
    {
        SaveToPrefs();
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetFloat("ScreenshakeAmountPref", screenshakeScaling);
        PlayerPrefs.SetInt("DisableFlashingPref", disableFlashing ? 1 : 0);
        PlayerPrefs.SetInt("SFXCaptionsPref", enableSFXCaptions ? 1 : 0);
        PlayerPrefs.SetInt("OneHandModePref", enableOneHandMode ? 1 : 0);
    }

    // ==============================================================
    // Public manipulators
    // ==============================================================

    public void SetScreenshake(float value)
    {
        screenshakeScaling = value;
        Settings.ScreenShakeScalar = screenshakeScaling;
    }

    public void UpdateScreenshakeText()
    {
        screenshakeReadout.text = $"{(int) (screenshakeScaling * 100)}%";
    }

    public void SetDisableFlashing(bool value)
    {
        disableFlashing = value;
        Settings.FlashingDisabled = value;
    }

    public void SetSFXCaptions(bool value)
    {
        enableSFXCaptions = value;
        Settings.SFXCaptionsEnabled = value;
    }

    public void SetOneHandMode(bool value)
    {
        enableOneHandMode = value;
        Settings.OneHandMode = value;
    }
}
