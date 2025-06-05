using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ChoiceSelectionFlair : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image leafSelectionHighlight;
    public TMP_Text text;
    public bool isDefaultSelection = false;

    public AudioEvent selectionHighlightSFX;

    private bool FirstEnable = false;

    private void OnEnable() {
        if (isDefaultSelection && FirstEnable == false) {
            leafSelectionHighlight.gameObject.SetActive(true);
            text.color = Color.black;
            FirstEnable = true;
        }
    }

    public void OnDeselect(BaseEventData eventData) {
        leafSelectionHighlight.gameObject.SetActive(false);
        text.color = Color.white;
    }

    public void OnSelect(BaseEventData eventData) {
        leafSelectionHighlight.gameObject.SetActive(true);
        text.color = Color.black;

        selectionHighlightSFX.Play(gameObject);
    }
}
