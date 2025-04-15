using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUi : MonoBehaviour
{
    // Parameters =================================================================================

    [Header("Shared Dependency")]
    public Canvas NavBar;

    // Dialogue
    [Header("Dependencies")]
    public Canvas Default;
    public Canvas Pause;
    public Canvas Options;
    public Canvas Controls;
    public Canvas Bartering;
    public BarteringController BarteringController;
    public Canvas Dialogue;
    public DialogueUiManager DialogueUiManager;
    public NotificationUI Notification;

    public enum UiStates {
        Default,
        Pause,
        Options, 
        MoveToTitle,
        Controls,
        Bartering,
        Dialogue
    }

    [SerializeField, ReadOnly] private UiStates _currentCanvasState;
    public System.Action<UiStates, UiStates> CanvasStateChanged;

    [Header("Audio")]
    [SerializeField] private AK.Wwise.Event pauseOpen;
    [SerializeField] private AK.Wwise.Event pauseClose;

    public UiStates CurrentCanvasState {
        get { 
            return _currentCanvasState; 
        }
        private set { 
            _currentCanvasState = value;
            CanvasStateChanged?.Invoke(_currentCanvasState, value);
        }
    }

    // Initializers and Update ================================================================
    void Start() {
        if (Default == null) {
            Debug.LogError("Default Canvas dependency not set.");
        }

        // Swap with Accessibility Check
        MoveToDefault();
    }
    
    /// <summary>
    /// Check for player input and update UI.
    /// </summary>
    public void Update() {

        if(GameManager.PlayerInput.GetMenu1Down() || GameManager.PlayerInput.GetStartDown()) {
            // NOTE: REPLACED INVENTORY CODE WITH PAUSE
            if (CurrentCanvasState == UiStates.Pause || CurrentCanvasState == UiStates.Controls || CurrentCanvasState == UiStates.Options)
            {
                MoveTo(UiStates.Default);
            }
            else
            {
                if (CurrentCanvasState == UiStates.Dialogue || CurrentCanvasState == UiStates.Bartering) return;
                MoveToPause();
                pauseOpen.Post(this.gameObject);
            }
        }
    }

    // Public Utility Methods ====================================================================

    /// <summary>
    /// Transition Start Ui to a new state.
    /// </summary>
    /// <param name="newState"> State to move to. </param>
    public void MoveTo(UiStates newState) {
        StopState(CurrentCanvasState);
        StartState(newState);
    }

    // Used for button OnClick calls as they don't let enums to be passed through :|
    public void MoveToDefault() => MoveTo(UiStates.Default);
    public void MoveToPause() => MoveTo(UiStates.Pause);
    public void MoveToOptions() => MoveTo(UiStates.Options);
    public void MoveToTitle() => MoveTo(UiStates.MoveToTitle);
    public void MoveToControls() => MoveTo(UiStates.Controls);
    public void MoveToDialogue() => MoveTo(UiStates.Dialogue);
    public void MoveToBartering(BarteringController.TradeData tradeData) {
       
        BarteringController.InitializeTrade(tradeData);

        MoveTo(UiStates.Bartering);
    }

    

    // Private Helper Methods ====================================================================

    /// <summary>
    /// Stop a currently running Ui state.
    /// </summary>
    /// <param name="stateToStop"> State that will stop. </param>
    void StopState(UiStates stateToStop) {

        // Can't stop transition states
        // (MoveToTitle)

        switch (stateToStop) {
            case UiStates.Default:
                // Insert animation!
                GameManager.Player.Movement.SetCanMove(false);
                Default.gameObject.SetActive(false);
                break;
            case UiStates.Pause:
                // Insert animation!
                Pause.gameObject.SetActive(false);
                TimeLoopManager.SetLoopPaused(false);
                break;
            case UiStates.Options:
                // Insert animation!
                Options.gameObject.SetActive(false);
                TimeLoopManager.SetLoopPaused(false);
                break;
            case UiStates.Controls:
                // Insert animation!
                Controls.gameObject.SetActive(false);
                TimeLoopManager.SetLoopPaused(false);
                break;
            case UiStates.Bartering:
                // Insert animation!
                Bartering.gameObject.SetActive(false);
                break;
            case UiStates.Dialogue:
                // Insert animation!

                GameManager.DialogueManager.StopMidDialogue();
                
                Dialogue.gameObject.SetActive(false);
                break;
        }

    }

    /// <summary>
    /// Start a new state.
    /// </summary>
    /// <param name="stateToStart">State that will start.</param>
    void StartState(UiStates stateToStart) {

        // Previous state
        // this is mainly so the unpause sound doesn't play on startup lol
        UiStates previousState = CurrentCanvasState; 

        // Set our new state
        CurrentCanvasState = stateToStart;

        switch (stateToStart) {
            case UiStates.Default:
                // Insert animation!
                GameManager.Player.Movement.SetCanMove(true);
                GameManager.Player.InteractionHandler.SetCanInteract(true);
                if (previousState != UiStates.Default) pauseClose.Post(this.gameObject); // play menu close sound only on unpause
                Default.gameObject.SetActive(true);
                break;
            case UiStates.Pause:
                // Insert animation!
                GameManager.Player.Movement.SetCanMove(false);
                GameManager.Player.InteractionHandler.SetCanInteract(false);
                Pause.gameObject.SetActive(true);
                TimeLoopManager.SetLoopPaused(true);
                break;
            case UiStates.Options:
                // Insert animation!
                Options.gameObject.SetActive(true);
                TimeLoopManager.SetLoopPaused(true);
                break;
            case UiStates.MoveToTitle:
                // Insert animation!
                SceneManager.LoadScene(0);
                break;
            case UiStates.Controls:
                // Insert animation!
                Controls.gameObject.SetActive(true);
                TimeLoopManager.SetLoopPaused(true);
                break;
            case UiStates.Bartering:
                GameManager.Player.Movement.SetCanMove(false);
                GameManager.Player.InteractionHandler.SetCanInteract(false);
                Bartering.gameObject.SetActive(true);
                break;
            case UiStates.Dialogue:
                // Insert animation!
                GameManager.Player.Movement.SetCanMove(false);
                GameManager.Player.InteractionHandler.SetCanInteract(false);
                Dialogue.gameObject.SetActive(true);
                break;
        }
    }
}
