using TMPro;
using UnityEngine;

public class AutoUiScaler : MonoBehaviour {
    public TMP_Text TextForScale;
    public RectTransform RectTransform;
    public float HorizontalPadding = 50;
    public float VerticalPadding = 50;
    public Vector2 MinSize;
    public float LineLength = 700;

    private bool _readyToRescale = false;
    private TMP_TextInfo _info;
    private Vector3 StartPos = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        _info = TextForScale.textInfo;
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ScaleToText);
        StartPos = RectTransform.transform.localPosition;
    }

    private void OnDisable() {
        RectTransform.sizeDelta = new Vector2(HorizontalPadding, VerticalPadding);

        if (StartPos != Vector3.zero) {
            RectTransform.transform.localPosition = StartPos;
        }
    }

    /// <summary>
    /// Scale a Rect Transform to a text size.
    /// </summary>
    /// <param name="obj"> The TMP_Text that has updated. </param>
    void ScaleToText(Object obj) {
        if (obj != TextForScale) return;
        if (_readyToRescale) return;

        _readyToRescale = true;
    }
    private void Update() {

        if (_readyToRescale == false) return;

        if (_info.lineCount == 0) return;
        float textWidth = _info.lineInfo[_info.lineCount - 1].width;
        float textActualWidth = TextForScale.renderedWidth;
        float textHeight = TextForScale.renderedHeight;

        if(textActualWidth < MinSize.x)
        {
            textActualWidth = MinSize.x;
        }

        if (textHeight < MinSize.y)
        {
            textHeight = MinSize.y;
        }

        //RectTransform.transform.localPosition += new Vector3(0, textHeight + _info.lineInfo[_info.lineCount - 1].lineHeight, 0);

        if (textWidth >= 1) {
           RectTransform.sizeDelta = new Vector2(textActualWidth + HorizontalPadding, textHeight + VerticalPadding);
        }
    }
}
