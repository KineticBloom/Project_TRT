using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputRebindManager : MonoBehaviour
{
    [SerializeField] private GameObject hintScreen;
    
    private PlayerControls _controls => GameManager.PlayerInput.PlayerControls;
    private InputControlScheme _lastScheme => GameManager.PlayerInput.LastUsedScheme;
    private UnityEvent _inputEvent => GameManager.PlayerInput.OnInputSchemeChanged;

    #region Public Methods
    public void Rebind0(InputActionReference actionRef) => Rebind(_controls.FindAction(actionRef.action.name), 0);
    public void Rebind1(InputActionReference actionRef) => Rebind(_controls.FindAction(actionRef.action.name), 1);
    
    public void RebindUp() => Rebind(_controls.MainControls.ControlAxis, 1);
    public void RebindLeft() => Rebind(_controls.MainControls.ControlAxis, 2);
    public void RebindDown() => Rebind(_controls.MainControls.ControlAxis, 3);
    public void RebindRight() => Rebind(_controls.MainControls.ControlAxis, 4);
    
    public void ResetUp() => RemoveControlAxisBinding(1);
    public void ResetLeft() => RemoveControlAxisBinding(2);
    public void ResetDown() => RemoveControlAxisBinding(3);
    public void ResetRight() => RemoveControlAxisBinding(4);
    
    public void ResetBind(InputActionReference actionRef)
    {
        _controls.FindAction(actionRef.action.name).RemoveAllBindingOverrides();
        _inputEvent?.Invoke();
    }
    public void ResetAllBindings()
    {
        _controls.RemoveAllBindingOverrides();
        _inputEvent?.Invoke();
    }
    #endregion
    
    #region Private Methods
    private void Rebind(InputAction action, int bindingIndex)
    {
        if (bindingIndex == 0) bindingIndex = action.GetBindingIndex(_lastScheme.bindingGroup);
        
        hintScreen.SetActive(true);
        _controls.MainControls.Disable();
        
        InputBinding oldBinding = action.bindings[bindingIndex];
        
        action.Disable();
        
        var rebind = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/Tab")
            .WithCancelingThrough("<Gamepad>/Start")
            .WithBindingGroup(_lastScheme.bindingGroup)
            .OnCancel(op => {
                hintScreen.SetActive(false);
                action.Enable();
                _controls.MainControls.Enable();
                op.Dispose();
            })
            .OnComplete(op => {
                CheckOtherBinds(action, bindingIndex, oldBinding);
                hintScreen.SetActive(false);
                action.Enable();
                _controls.MainControls.Enable();
                _inputEvent?.Invoke();
                op.Dispose();
            });
        rebind.Start();
    }
    
    private void CheckOtherBinds(InputAction action, int bindingIndex, InputBinding oldBinding)
    {
        InputBinding currBinding = action.bindings[bindingIndex];
        foreach (InputBinding binding in action.actionMap.bindings)
        {
            if (binding.action == currBinding.action) continue;
            else if (binding.effectivePath == currBinding.effectivePath)
            {
                if (ActionRebindable(binding.action)) action.actionMap[binding.action].ApplyBindingOverride(bindingIndex, oldBinding.effectivePath);
                else action.ApplyBindingOverride(bindingIndex, oldBinding.effectivePath);
            }
        }
        
        for (int i = 0; i < action.bindings.Count; i++)
        {
            if (i == bindingIndex) continue;
            if (action.bindings[i].effectivePath == currBinding.effectivePath) action.ApplyBindingOverride(i, oldBinding.effectivePath);
        }
    }
    
    private bool ActionRebindable(string actionName) 
    {
        return actionName switch
        {
            "AffirmButton" => true,
            "RejectButton" => true,
            "PrimaryTrigger" => true,
            "MenuButton1" => true,
            _ => false,
        };
    }
    
    private void RemoveControlAxisBinding(int ind)
    {
        _controls.MainControls.ControlAxis.RemoveBindingOverride(ind);
        _inputEvent?.Invoke();
    }
    #endregion
}