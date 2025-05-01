using Cinemachine;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Processes Ink file and controls conversation flow.
/// </summary>
public class LimboDialogue : MonoBehaviour {

    #region ======== [ OBJECT REFERENCES ] ========

    public GameObject NPCDialogueBubblePrefab;
    public GameObject PlayerDialogueBubblePrefab;

    public System.Action EndCallback;

    public float TypeSpeed;

    #endregion

    #region ======== [ PUBLIC PROPERTIES ] ========

    public struct LineData {

        public string GoalLine;
        public int CharactersPrinted;

        public bool SaidByNPC;
        public bool TriggersBarter;
        public bool LineHasChoices;
        public List<Choice> Choices;

    }

    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========

    private SpeechBubbleCore NPCBubble;
    private SpeechBubbleCore PlayerBubble;
    private Story CurrentStory;
    private bool InDialogue;
    private SpeechBubbleCore CurrentBubble;
    private LineData CurrentLineData;
    private DialogueUiManager DialogueUiManager;

    private bool LineFinished = true;
    private bool _onDelay = false;
    private bool _noInput = false;
    private bool _externalNoInput = false;

    public delegate void CallBackBarterTrigger();

    private CallBackBarterTrigger callBackBarterTrigger; // to store the function

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Function to pause dialogue interactions from player.
    /// </summary>
    /// <param name="FreezeDialogue">True then no input accepted.</param>
    public void FreezeDialogue(bool FreezeDialogue) {
        Debug.Log("Set Dialogue Freeze to: " + FreezeDialogue);
        _externalNoInput = FreezeDialogue;
    }

    /// <summary>
    /// Stop current dialogue mid conversation if needed.
    /// </summary>
    public void StopMidDialogue() {

        if (InDialogue == false) return;

        Debug.Log("Stopped mid dialogue!");

        // Reset trackers
        StopAllCoroutines();
        
        InDialogue = false;
        _noInput = false;
        CurrentStory = null;

        // Destroy Dependencies
        if (NPCBubble != null) {
            Destroy(NPCBubble.gameObject);
        }

        if (PlayerBubble != null) {
            Destroy(PlayerBubble.gameObject);
        }

        if (DialogueUiManager != null) {
            DialogueUiManager.HideButtons();
        }

        // External Setup
        if (TimeLoopManager.Instance != null) TimeLoopManager.SetLoopPaused(false);

        // Start delay
        _onDelay = false;
    }

    /// <summary>
    /// Start Dialogue from given INK file
    /// </summary>
    /// <param name="DialogueINKFile"> Dialogue to display</param>
    /// <param name="SkipToINKKnot"> INK Knot to jump too</param>
    public void StartDialogue(TextAsset DialogueINKFile, string SkipToINKKnot = "NONE") {

        if (InDialogue) return;
        if (_onDelay) return;

        InDialogue = true;

        // Setup Systems
        SetupUi();
        SetupDialogue(DialogueINKFile, SkipToINKKnot);

        // Start next line
        SetupNextLine();
    }



    /// <summary>
    /// Used by inspector buttons to select a choice.
    /// </summary>
    /// <param name="ChoiceIndex">Index of the chosen choice.</param>
    public void InspectorChooseChoice(int ChoiceIndex) {
        CurrentStory.ChooseChoiceIndex(ChoiceIndex);

        DialogueUiManager.HideButtons();
        StartCoroutine(InputDelay());
        SetupNextLine();
    }

    #endregion

    #region ======== [ UPDATE ] ========

    private void Update() {

        if (InDialogue == false) return;

        if (_onDelay) return;
        if (_noInput) return;
        if (_externalNoInput) return;

        // Check for inputs
        if (GameManager.PlayerInput.GetAffirmDown() || GameManager.PlayerInput.GetClickDown()) {
            if (LineFinished == false) {
                SkipToEndOfLine();
            } else {
                if (EndStoryIfPossible()) {
                    return;
                }
                SetupNextLine();
            }
            return;
        }

        // JUST expedite
        if (GameManager.PlayerInput.GetPrimaryTriggerDown()) {
            SkipToEndOfLine();
            return;
        }

        // JUST continue
        if (GameManager.PlayerInput.GetSecondaryTriggerDown()) {
            if (LineFinished) {
                if (EndStoryIfPossible()) {
                    return;
                }
                SetupNextLine();
                return;
            }

        }
    }

    #endregion

    #region ======== [ SETUP METHODS ] ========

    /// <summary>
    /// Create Dialogue Bubbles for NPC and Player.
    /// </summary>
    /// <param name="NPCWorldPosition">World position of NPC.</param>
    /// <param name="PlayerWorldPosition">World position of Player.</param>
    private void SetupUi() {
        Transform BubbleParent = DialogueUiManager.ParentForDialogueBubbles.transform;

        // Create NPC Dialogue Bubble
        GameObject NPCBubbleObject = Instantiate(NPCDialogueBubblePrefab, new Vector3(0,0), Quaternion.identity, BubbleParent);
        NPCBubble = NPCBubbleObject.GetComponent<SpeechBubbleCore>();

        // Create Player Dialogue Bubble
        GameObject PlayerBubbleObject = Instantiate(PlayerDialogueBubblePrefab, new Vector3(0, 0), Quaternion.identity, BubbleParent);
        PlayerBubble = PlayerBubbleObject.GetComponent<SpeechBubbleCore>();
    }

    /// <summary>
    /// Initalize story and parse Ink variables.
    /// </summary>
    private void SetupDialogue(TextAsset DialogueINKFile, string SkipToINKKnot = "NONE") {

        // Create story
        CurrentStory = new Story(DialogueINKFile.text);

        // Skip to specific KNOT if wanted
        if (SkipToINKKnot != "NONE") {
            CurrentStory.ChoosePathString(SkipToINKKnot);
        }

        // Parse INK Variables
        System.Action inkyVars = null;

        foreach (string id in CurrentStory.variablesState) {
            inkyVars += () => {
                CurrentStory.variablesState[id] = GameManager.FlagTracker.ExtractFlag(id);
                CurrentStory.ObserveVariable(id, (string varName, object newValue) => GameManager.FlagTracker.SetFlag(varName, newValue));
            };
        }
        inkyVars?.Invoke();
        inkyVars = null;
    }

    /// <summary>
    /// Setup to print the next line of dialogue.
    /// </summary>
    private void SetupNextLine() {

        // Gates
        if (CurrentStory == null) return;
        if (EndStoryIfPossible()) return;
        if (!CurrentStory.canContinue) return;

        // Get next line
        string NextLine = CurrentStory.Continue();

        // Get line data
        CurrentLineData = ProcessTags(CurrentStory.currentTags);
        CurrentLineData.GoalLine = NextLine;

        // Trigger barter?
        if (CurrentLineData.TriggersBarter) {
            EndStory();
            if (callBackBarterTrigger != null) {
                callBackBarterTrigger();
            }
            return;
        }

        NPCBubble.gameObject.SetActive(false);
        PlayerBubble.gameObject.SetActive(false);

        // Choose Bubble
        if (CurrentLineData.SaidByNPC) {
            CurrentBubble = NPCBubble;
        } else {
            CurrentBubble = PlayerBubble;
        }

        // Setup Line
        CurrentBubble.TMPText.text = "";
        CurrentBubble.gameObject.SetActive(true);
        LineFinished = false;

        StartCoroutine(PrintNextCharacter());
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// End Story.
    /// </summary>
    private void EndStory() {

        // Reset trackers
        InDialogue = false;
        CurrentStory = null;

        // Destroy Dependencies
        Destroy(NPCBubble.gameObject);
        Destroy(PlayerBubble.gameObject);

        // Start delay
        _onDelay = true;
        StartCoroutine(ConversationDelay());

        // TODO: Send to main scene (and preload it at the start)
    }

    /// <summary>
    /// Check if we can end the story.
    /// </summary>
    /// <returns>True if story can be ended. False if not.</returns>
    private bool CanEndStory() {

        if(CurrentStory == null) { return false; }

        bool CanContinue = CurrentStory.canContinue;
        bool HasChoices = (CurrentStory.currentChoices != null) && (CurrentStory.currentChoices.Count != 0);

        return (CanContinue == false) && (HasChoices == false);
    }

    /// <summary>
    /// Tries to end story, if can will.
    /// </summary>
    /// <returns>True if story ended, false otherwise.</returns>
    private bool EndStoryIfPossible() {
        if (CanEndStory()) {
            EndStory();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Convert ink tags to a ProcessedTag struct.
    /// </summary>
    /// <param name="INKLineTags">Ink tags</param>
    /// <returns>Our new ProcessedTag struct.</returns>
    LineData ProcessTags(List<string> INKLineTags) {

        LineData foundTags = new LineData();

        if (INKLineTags == null) {
            return foundTags;
        }

        foreach (string tag in INKLineTags) {
            // Get current tag key and value
            string[] tagSplit = tag.Split(":");
            string key = tagSplit[0];
            string value = tagSplit.Length == 2 ? tagSplit[1] : "";
            key = key.ToLower();

            // Process Tag
            switch (key) {
                case "npc":
                    foundTags.SaidByNPC = true;
                    break;
                case "barter":
                    foundTags.TriggersBarter = true;
                    break;
            }
        }

        foundTags.LineHasChoices = CurrentStory.currentChoices.Count > 0;
        foundTags.Choices = CurrentStory.currentChoices;
        foundTags.CharactersPrinted = 0;

        return foundTags;
    }

    /// <summary>
    /// Skip text animation and print full line.
    /// </summary>
    private void SkipToEndOfLine() {
        StopCoroutine(PrintNextCharacter());
        CurrentBubble.TMPText.text = CurrentLineData.GoalLine;
        CurrentLineData.CharactersPrinted = CurrentLineData.GoalLine.Length - 1;
        LineFinished = true;

        if (CurrentLineData.LineHasChoices) {
            DialogueUiManager.ShowButtons(CurrentLineData.Choices);
        }
    }

    /// <summary>
    /// Print next character in line.
    /// </summary>
    IEnumerator PrintNextCharacter() {

        // Find current char
        int TextLength = CurrentLineData.GoalLine.Length;
        int CharactersPrinted = CurrentLineData.CharactersPrinted;
        int NextCharacterIndex = Mathf.Clamp(CharactersPrinted, 0, TextLength - 1);

        char NextCharacter = CurrentLineData.GoalLine[NextCharacterIndex];

        // Wait for correct length
        float ActualTextSpeed = TypeSpeed;

        if (NextCharacter == '.') {
            ActualTextSpeed *= 5;
        }

        yield return new WaitForSeconds(ActualTextSpeed);

        // Play sound every other character or if a punctuation
        if (NextCharacter == '.' || CharactersPrinted % 2 == 0) {
            //playTalkSound(currentCharacter);
        }

        if (CurrentLineData.CharactersPrinted > CharactersPrinted) {
            yield break;
        }

        CurrentBubble.TMPText.text += NextCharacter;
        CurrentLineData.CharactersPrinted += 1;

        if (CurrentLineData.CharactersPrinted >= TextLength) {

            // Stop printing!
            if (CurrentLineData.LineHasChoices) {
                DialogueUiManager.ShowButtons(CurrentLineData.Choices);
            }
            LineFinished = true;

        } else {
            StartCoroutine(PrintNextCharacter());
        }
    }

    /// <summary>
    /// Delay Input to prevent Input Ghosting
    /// </summary>
    /// <returns></returns>
    IEnumerator InputDelay() {
        _noInput = true;
        yield return 0;
        _noInput = false;
    }

    /// <summary>
    /// Delay Conversation for Input Ghosting
    /// </summary>
    /// <returns></returns>
    IEnumerator ConversationDelay() {
        yield return new WaitForSeconds(0.25f);
        _onDelay = false;
        EndCallback?.Invoke(); // Messy code
    }

    #endregion
}
