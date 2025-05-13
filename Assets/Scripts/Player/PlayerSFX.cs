using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [SerializeField]
    public AudioEvent playerFootstepSFX;

    public void PlayPlayerFootstepSFX()
    {
        // Play
        playerFootstepSFX.Play(gameObject);
    }
}
