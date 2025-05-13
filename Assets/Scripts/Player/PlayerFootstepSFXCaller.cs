using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I had to play the footstep sfx in a different script attached to the player instead of here to make reverbs work
public class PlayerFootstepSFXCaller : MonoBehaviour
{
    [SerializeField]
    public PlayerSFX _playerSFXObject;

    void PlayPlayerFootstepSFX()
    {
        // Play
        if (_playerSFXObject)
        {
            _playerSFXObject.PlayPlayerFootstepSFX();
        }
    }
}
