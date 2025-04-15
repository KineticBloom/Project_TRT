using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Ink.Runtime;
using System.Collections;

public class DialogueUiManager : MonoBehaviour {

    #region ======== [ PUBLIC PROPERTIES ] ========

    public List<Button> UiButtons;
    public GameObject ParentForDialogueBubbles;

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Show and setup Dialogue Buttons
    /// </summary>
    /// <param name="choices"> Choices array from INK to display in choices. </param>
    public void ShowButtons(List<Choice> choices) {

        // Reset buttons
        HideButtons();

        // Display all choices
        for (int i = 0; i < choices.Count; i++) {

            Button Button = UiButtons[i];
            Button.gameObject.SetActive(true);

            TMP_Text TextContainer = Button.GetComponentInChildren<TMP_Text>();
            TextContainer.text = choices[i].text;
        }
    }

    /// <summary>
    /// Hide buttons from canvas.
    /// </summary>
    public void HideButtons() {
        foreach (Button button in UiButtons) {
            button.gameObject.SetActive(false);
        }
    }

    public void ChooseButtonOne() {
        GameManager.DialogueManager.InspectorChooseChoice(0);
    }

    public void ChooseButtonTwo() {
        GameManager.DialogueManager.InspectorChooseChoice(1);
    }

    public void ChooseButtonThree() {
        GameManager.DialogueManager.InspectorChooseChoice(2);
    }

    public void ChooseButtonFour() {
        GameManager.DialogueManager.InspectorChooseChoice(3);
    }

    #endregion
}
