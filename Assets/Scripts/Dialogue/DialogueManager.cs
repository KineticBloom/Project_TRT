using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Processes Ink file and controls conversation flow.
/// </summary>
public class DialogueManager : MonoBehaviour {

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
    private Canvas CurrentCanvas;
    private Camera CurrentCamera;
    private Story CurrentStory;
    private bool InDialogue;
    private SpeechBubbleCore CurrentBubble;
    private LineData CurrentLineData;
    private InGameUi InGameUi;
    private DialogueUiManager DialogueUiManager;

    private bool LineFinished = true;
    private bool _onDelay = false;
    private bool _noInput = false;

    public delegate void CallBackBarterTrigger();

    private CallBackBarterTrigger callBackBarterTrigger; // to store the function

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Start Dialogue from given INK file
    /// </summary>
    /// <param name="DialogueINKFile"> Dialogue to display</param>
    /// <param name="NPCWorldPosition"> Position for NPC Dialogue Bubble</param>
    /// <param name="PlayerWorldPosition"> Position for Player Dialogue Bubble </param>
    /// <param name="SkipToINKKnot"> INK Knot to jump too</param>
    public void StartDialogue(TextAsset DialogueINKFile, CallBackBarterTrigger callBackBarterTrigger, Vector3 NPCWorldPosition, Vector3 PlayerWorldPosition, string SkipToINKKnot = "NONE") {

        if (InDialogue) return;
        if (_onDelay) return;
        InDialogue = true;

        bool FoundDependencies = SetupDependencies();
        if (FoundDependencies == false) return;

        this.callBackBarterTrigger = callBackBarterTrigger;

        // Pause Game
        InGameUi.MoveToDialogue();
        TimeLoopManager.SetLoopPaused(true);

        // Setup Systems
        SetupUi(NPCWorldPosition, PlayerWorldPosition);
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
    /// Find InGameUi and DialogueUiManager in current scene.
    /// </summary>
    /// <returns> True if dependencies found! False if not. </returns>
    private bool SetupDependencies() {
        InGameUi = GameManager.MasterCanvas.gameObject.GetComponent<InGameUi>();
        if (InGameUi == null) return false;

        DialogueUiManager = InGameUi.DialogueUiManager;
        if (DialogueUiManager == null) return false;

        return true;
    }

    /// <summary>
    /// Create Dialogue Bubbles for NPC and Player.
    /// </summary>
    /// <param name="NPCWorldPosition">World position of NPC.</param>
    /// <param name="PlayerWorldPosition">World position of Player.</param>
    private void SetupUi(Vector3 NPCWorldPosition, Vector3 PlayerWorldPosition) {

        CurrentCanvas = GameManager.MasterCanvas;
        CurrentCamera = Camera.main;

        Transform BubbleParent = DialogueUiManager.ParentForDialogueBubbles.transform;

        // Create NPC Dialogue Bubble
        Vector2 NPCViewportPosition = WorldPosToViewportPos(NPCWorldPosition, CurrentCanvas, CurrentCamera);
        GameObject NPCBubbleObject = Instantiate(NPCDialogueBubblePrefab, NPCViewportPosition, Quaternion.identity, BubbleParent);
        NPCBubble = NPCBubbleObject.GetComponent<SpeechBubbleCore>();

        // Create Player Dialogue Bubble
        Vector2 PlayerViewportPosition = WorldPosToViewportPos(PlayerWorldPosition, CurrentCanvas, CurrentCamera);
        GameObject PlayerBubbleObject = Instantiate(PlayerDialogueBubblePrefab, PlayerViewportPosition, Quaternion.identity, BubbleParent);
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
                CurrentStory.variablesState[id] = GameManager.FlagTracker.CheckFlag(id);
                CurrentStory.ObserveVariable(id, (string varName, object newValue) => GameManager.FlagTracker.SetFlag(varName, (bool)newValue));
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
    /// Take 3D world position and map it to 2D Camera viewport.
    /// </summary>
    /// <param name="WorldPos">Position to convert.</param>
    /// <returns>Relative viewport position.</returns>
    private Vector2 WorldPosToViewportPos(Vector3 WorldPos, Canvas CanvasToUse, Camera CameraToUse) {
        Vector3 viewportPos = CameraToUse.WorldToViewportPoint(WorldPos);
        Vector2 canvasResolution = CanvasToUse.GetComponent<CanvasScaler>().referenceResolution;
        return new Vector2(viewportPos.x * canvasResolution.x, viewportPos.y * canvasResolution.y);
    }

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

        // External Setup
        TimeLoopManager.SetLoopPaused(false);
        InGameUi.MoveToDefault();

        // Start delay
        _onDelay = true;
        StartCoroutine(ConversationDelay());  
    }

    /// <summary>
    /// Check if we can end the story.
    /// </summary>
    /// <returns>True if story can be ended. False if not.</returns>
    private bool CanEndStory() {

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
        CurrentLineData.CharactersPrinted = CurrentLineData.GoalLine.Length-1;
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
