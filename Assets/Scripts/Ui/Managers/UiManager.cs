using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Contains each Ui States preferences for game environment.
/// </summary>
[Serializable]
public struct StateData {
    public CanvasGroup StatesCanvasGroup;
    public bool IsPlayerFrozen;
    public bool IsTimeFrozen;
    public bool IsDialogueFrozen;
    public bool PauseAnimations;
    public bool DisableInventoryInteractions;
}

/// <summary>
/// Base Ui Manager for generic references without UiStateEnum param
/// </summary>
public abstract class UiManagerBase : MonoBehaviour {

    // Is this UIManager the current focus?
    bool _currentFocus;

    // Public get and set to call below functions
    public bool CurrentFocus {
        get {
            return _currentFocus;
        }
        set {
            _currentFocus = value;

            if (value == false) {
                DisableFocus();
            } else {
                EnableFocus();
            }
        }
    }

    protected abstract void EnableFocus();
    protected abstract void DisableFocus();

}

/// <summary>
/// Handles internal state swapping and external UiManager swapping!
/// </summary>
/// <typeparam name="UiStateEnum">All UI states tracked by manager.</typeparam>
public abstract class UiManager<UiStateEnum> : UiManagerBase where UiStateEnum : Enum {

    #region ======== [ PARAMETERS ] ========

    public UiStateEnum DefaultState;

    #endregion

    #region ======== [ PRIVATE PROPERTY ] ========

    protected UiStateEnum _currentState;
    protected StateData _currentStateData;
    private GameObject _lastSelection;
    private bool _noStateAssigned = true;
    private Stack<UiStateEnum> _lastStates = new();
    private Stack<GameObject> _lastButtons = new();
    private bool _back = true;

    /// Serialized and setup in inspector. Allows designer to setup each state data.
    [SerializeField]
    List<Pair<UiStateEnum, StateData>> states = new List<Pair<UiStateEnum, StateData>>();

    #endregion

    #region ======== [ PUBLIC METHOD ] ========

    /// <summary>
    /// Move to a given state
    /// </summary>
    /// <param name="newState">State to move to</param>
    public void MoveTo(UiStateEnum newState) {

        // Find matching stateData, then move to state
        foreach (Pair<UiStateEnum, StateData> x in states) {
            if (x.First.Equals(newState)) {

                // Change state
                ChangeState(newState, x.Second, _currentState, _currentStateData);

                // Set current data
                _currentState = newState;
                _currentStateData = x.Second;
                return;
            }
        }

        // If no matching stateData...
        Debug.LogError("Tried to swap to " + newState.ToString() + " but no StateData setup for that state on this Manager");
    }

    /// <summary>
    /// Returns to the last UI State if any
    /// </summary>
    public virtual void GoBack()
    {
        if (!_lastStates.TryPop(out UiStateEnum lastState)) return;
        _back = true;
        MoveTo(lastState);
        if (_lastButtons.TryPop(out GameObject button) && button.TryGetComponent(out Button button1))
        {
            button1.Select();
        }
    }

    void Update()
    {
        if (!CurrentFocus) return;
        if (GameManager.PlayerInput.GetRejectDown()) GoBack();
    }

    #endregion

    #region ======== [ PRIVATE METHOD ] ========
    /// <summary>
    /// Internal logic for switching states.
    /// </summary>
    /// <param name="newState"> State to switch to </param>
    /// <param name="newStateData"> States to switch to data </param>
    /// <param name="oldState"> State you are leaving </param>
    /// <param name="oldStateData"> State you are leaving data </param>
    protected void ChangeState(UiStateEnum newState, StateData newStateData, UiStateEnum oldState, StateData oldStateData) {

        // Hide old state
        if (oldStateData.StatesCanvasGroup != null) {
            oldStateData.StatesCanvasGroup.gameObject.SetActive(false);
        }
        
        if (!_back)
        {
            _lastStates.Push(oldState);
            _lastButtons.Push(EventSystem.current.currentSelectedGameObject);
        }
        _back = false;

        // Show new state
        newStateData.StatesCanvasGroup.gameObject.SetActive(true);

        // Load new properties
        if (GameManager.Player != null) {
            GameManager.Player.Movement.SetCanMove(!newStateData.IsPlayerFrozen);
            GameManager.Player.InteractionHandler.SetCanInteract(!newStateData.IsPlayerFrozen);
        }

        if (GameManager.DialogueManager != null)
        {
            GameManager.DialogueManager.FreezeDialogue(newStateData.IsDialogueFrozen);
        }

        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(newStateData.IsTimeFrozen);
        }

        if (newStateData.PauseAnimations) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }

        if (newStateData.DisableInventoryInteractions)
        {
            GameManager.Instance.SetInventoryBarInteractable(false);
        } else
        {
            GameManager.Instance.SetInventoryBarInteractable(true);
        }
    }

    protected void LoadState(UiStateEnum stateToLoad) {

        StateData stateData = new StateData();
        bool foundData = false;

        foreach (Pair<UiStateEnum, StateData> x in states) {
            if (x.First.Equals(stateToLoad)) {
                stateData = x.Second;
                foundData = true;
            }
        }

        if (foundData == false) return;

        // Debug.Log("Load state: " + stateToLoad.ToString()); 

        // Load new properties
        if (GameManager.Player != null) {
            GameManager.Player.Movement.SetCanMove(!stateData.IsPlayerFrozen);
            GameManager.Player.InteractionHandler.SetCanInteract(!stateData.IsPlayerFrozen);
        }

        if (GameManager.DialogueManager != null)
        {
            GameManager.DialogueManager.FreezeDialogue(stateData.IsDialogueFrozen);
        }

        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(stateData.IsTimeFrozen);
        }

        if (stateData.PauseAnimations) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Set if our UIStates canvases can be interacted with.
    /// </summary>
    private void SetInteraction(bool CanInteract) {
        this.gameObject.SetActive(true);
        foreach (Pair<UiStateEnum, StateData> x in states) {
            x.Second.StatesCanvasGroup.interactable = CanInteract;
        }
    }

    /// <summary>
    /// Focus on this Ui Manager.
    /// </summary>
    protected override void EnableFocus() {

        SetInteraction(true);

        if (_noStateAssigned) {
            _back = true;
            MoveTo(DefaultState);
            _noStateAssigned = false;
        }

        // Show current state Canvas
        _currentStateData.StatesCanvasGroup.gameObject.SetActive(true);

        // Load old data
        if (GameManager.Player != null) {
            GameManager.Player.Movement.SetCanMove(!_currentStateData.IsPlayerFrozen);
            GameManager.Player.InteractionHandler.SetCanInteract(!_currentStateData.IsPlayerFrozen);
        }
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(_currentStateData.IsTimeFrozen);
        }

        if (_currentStateData.PauseAnimations) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }

        if (_lastSelection != null) {
            Button possibleButtion = _lastSelection.GetComponent<Button>();
            if (possibleButtion) {
                possibleButtion.Select();
            }
        }
    }

    /// <summary>
    /// Moving focus to another Ui Manager.
    /// </summary>
    protected override void DisableFocus() {
        _lastSelection = EventSystem.current.currentSelectedGameObject;
        SetInteraction(false);
        _lastButtons.Clear();
        _lastStates.Clear();
    }

    #endregion
}
