using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class BoldTextOnHover : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Text TextToBold;

    private void OnDisable() {
        TextToBold.fontStyle = FontStyles.Normal;
        TextToBold.text = TextToBold.text.Substring(0, 1).ToUpper() + TextToBold.text.Substring(1).ToLower();
    }

    public void OnDeselect(BaseEventData eventData) {
        TextToBold.fontStyle = FontStyles.Normal;
        TextToBold.text = TextToBold.text.Substring(0, 1).ToUpper() + TextToBold.text.Substring(1).ToLower();
    }

    public void OnSelect(BaseEventData eventData) {
        TextToBold.fontStyle = FontStyles.Bold;
        TextToBold.text = TextToBold.text.ToUpper();
    }

    public void TriggerBold() {
        TextToBold.fontStyle = FontStyles.Bold;
        TextToBold.text = TextToBold.text.ToUpper();
    }
}
