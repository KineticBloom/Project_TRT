using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class InputHints : MonoBehaviour
{
    private TextMeshProUGUI _display;
    private PlayerControls _controls => GameManager.PlayerInput.PlayerControls;
    private InputControlScheme _currentControl => GameManager.PlayerInput.LastUsedScheme;
    private string _originalText;

    private string InputSprite(string input) => $"<sprite name={_controls.FindAction(input).GetBindingDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames, _currentControl.bindingGroup).Split("|")[0].Trim()}>";
    private string MultiInputSprite(string input, int ind) => $"<sprite name={_controls.FindAction(input).GetBindingDisplayString(ind, InputBinding.DisplayStringOptions.DontUseShortDisplayNames)/* .Split("|")[ind].Trim() */}>";

    // Start is called before the first frame update
    void Awake()
    {
        _display = GetComponent<TextMeshProUGUI>();
        _originalText = _display.text;
    }

    void OnEnable()
    {
        StartCoroutine(EnableFunctions());
    }

    private IEnumerator EnableFunctions()
    {
        yield return null;
        SetInput();
        GameManager.PlayerInput.OnInputSchemeChanged.AddListener(SetInput);
    }

    void OnDisable()
    {
        GameManager.PlayerInput.OnInputSchemeChanged.RemoveListener(SetInput);
    }

    private void SetInput()
    {
        _display.spriteAsset = GameManager.PlayerInput.CurrentAsset();
        _display.text = _originalText;
        foreach (Match match in Regex.Matches(_originalText, @"\{(.*?)\}"))
        {
            string[] matches = match.Groups[1].Value.Split("_");
            string replacement = (matches.Length > 1) ? MultiInputSprite(matches[0], int.Parse(matches[1])) : InputSprite(match.Groups[1].Value);
            _display.text = _display.text.Replace(match.ToString(), replacement);
        }
    }
}
