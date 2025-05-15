using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InputSprite : MonoBehaviour
{
    private Image _display;

    // Start is called before the first frame update
    void Awake()
    {
        _display = GetComponent<Image>();
    }

    void OnEnable()
    {
        StartCoroutine(EnableFunctions());
    }

    private IEnumerator EnableFunctions()
    {
        yield return null;
        SetSprite();
        GameManager.PlayerInput.OnInputSchemeChanged.AddListener(SetSprite);
    }

    void OnDisable()
    {
        GameManager.PlayerInput.OnInputSchemeChanged.RemoveListener(SetSprite);
    }

    private void SetSprite()
    {
        _display.sprite = GameManager.PlayerInput.CurrentSprite();
    }
}
