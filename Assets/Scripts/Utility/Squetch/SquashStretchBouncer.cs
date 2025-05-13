using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

// ================================================================
//
// IMPORTANT NOTE:
// This script scales transforms from their PIVOT.
//
// If an objects's pivot is at its BOTTOM, like for the player and
// NPC prefabs, it will appear to squash and stretch from the 
// bottom of the object.
//
// If an object's pivot is at its CENTER, like by default, it will
// appear to squash and stretch from its center. 
// THIS MAY LOOK WRONG.
//
// ================================================================

public class SquashStretchBouncer : SquashStretchBrain
{
    // Parameters =================================================================================

    [Header("Bounce Parameters")]
    [SerializeField, Tooltip("The curve we evaluate when animating the bounce. "
                           + "The duration of the curve is determined by loopPeriod.\n\n"
                           + "Recall negative is squash and positive is stretch.")]
    private AnimationCurve bounceCurve = new(new Keyframe[]{
        // These points plot a sine curve.
        new(0, 0, 2*Mathf.PI, 2*Mathf.PI),
        new(0.25f, 1, 0, 0),
        new(0.5f, 0, -2*Mathf.PI, -2*Mathf.PI),
        new(0.75f, -1, 0, 0),
        new(1, 0, 2*Mathf.PI, 2*Mathf.PI),
    });

    [SerializeField, Tooltip("The duration, in seconds, of our looping bounce animation.\n\n"
                           + "Default: 2")]
    private float loopPeriod = 2;
    [SerializeField, Tooltip("The duration, in seconds, it takes to return to the base state when "
                           + "the animation stops.\n\nDefault: 0.025")]
    private float animKillTime = 0.025f;

    // Anim methods ===============================================================================

#if UNITY_EDITOR
    [Button]
#endif
    /// <summary>
    /// Starts the bounce animation.
    /// </summary>
    public void StartAnim()
    {
        StopAllCoroutines();
        StartCoroutine(EvaluateCurve(bounceCurve, loopPeriod));
    }

#if UNITY_EDITOR
    [Button]
#endif
    /// <summary>
    /// Stops the current bounce animation. Tweens our SquashStretch value back to 0.
    /// </summary>
    public void StopAnim()
    {
        StopAllCoroutines();
        // Return SquashStretch to 0 in animKillTime seconds.
        DOTween.To(() => SquashStretch, x => SquashStretch = x, 0, animKillTime);
    }

    private IEnumerator EvaluateCurve(AnimationCurve curve, float duration)
    {
        // Sets the value of squashStretch.squetch to the value evaluated from curve,
        // over duration seconds.
        // ================

        // Store the length of the animation as the time of the final keyframe.
        float animationLength = curve.keys[curve.length-1].time;

        while (true) {
            float elapsed = 0;

            while (elapsed < duration) {
                float evalAmount = elapsed/duration;
                SquashStretch = curve.Evaluate(evalAmount*animationLength);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }    
}