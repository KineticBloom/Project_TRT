using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubbleCore : MonoBehaviour
{
    [Header("Dependencies")]
    public TMP_Text TMPText;
    public Image Image;

    public void Hide() {
        gameObject.SetActive(false);
    }
    public void Show() {
        gameObject.SetActive(true);
    }
}
