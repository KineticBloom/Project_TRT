using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    #region ========== [ PARAMETERS ] ==========

    [ReadOnly] public bool IsOpen = false;

    [SerializeField] private AudioEvent doorOpenSFX;
    #endregion

    #region ========== [ PRIVATE PROPERTIES ] ==========

    private Animator _animator;

    #endregion

    #region ========== [ PUBLIC METHODS ] ==========

    public void OpenDoor()
    {
        _animator.SetTrigger("Open");

        if (!IsOpen)
        {
            doorOpenSFX.Play(gameObject);
        }

        IsOpen = true;

        LockedInteractable lockedInteractable = GetComponent<LockedInteractable>();
        if (lockedInteractable != null)
        {
            lockedInteractable.HideIcon = true;
            Destroy(lockedInteractable);
        }
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    #endregion
}
