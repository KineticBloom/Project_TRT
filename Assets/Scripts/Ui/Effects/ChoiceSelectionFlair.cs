using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ChoiceSelectionFlair : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Text text;
    public bool isDefaultSelection = false;

    private void OnEnable() {
        if (isDefaultSelection) {
            text.color = Color.white;
        }
    }

    private void OnDisable()
    {
        text.color = Color.black;
    }

    public void OnDeselect(BaseEventData eventData) {
        text.color = Color.black;
    }

    public void OnSelect(BaseEventData eventData) {
        //selectionHighlightSFX.Play(gameObject);
        text.color = Color.white;
    }
}
