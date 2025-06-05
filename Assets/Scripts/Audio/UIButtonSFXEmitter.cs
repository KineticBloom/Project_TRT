using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonSFXEmitter : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
{
    [SerializeField] private AudioEvent UIButtonHighlightSFX;
    [SerializeField] private AudioEvent UIButtonPressSFX;

    // To avoid a UI highlight sound playing right after a button press, cancel out the first ui sound 

    // Allow SFX Calls 
    public void OnSelect(BaseEventData eventData) {
        if (UIButtonHighlightSFX != null)
        {
            UIButtonHighlightSFX.Play(gameObject);
        }
    }

    public void OnDeselect(BaseEventData eventData) {
    }

    public void OnSubmit(BaseEventData eventData) {
        if (UIButtonPressSFX != null)
        {
            UIButtonPressSFX.Play(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (UIButtonPressSFX != null)
        {
            UIButtonPressSFX.Play(gameObject);
        }
    }
}
