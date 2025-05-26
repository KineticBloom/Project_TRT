using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquetchStarter : MonoBehaviour
{
    private SquashStretchBouncer SquashStretchBouncer;

    [Header("Fast Bounce Parameters")]
    [SerializeField, Tooltip("The curve we evaluate when animating the bounce. "
                           + "The duration of the curve is determined by loopPeriod.\n\n"
                           + "Recall negative is squash and positive is stretch.")]
    private AnimationCurve fastBounceCurve = new(new Keyframe[]{
        // These points plot a sine curve.
        new(0, 0, 2*Mathf.PI, 2*Mathf.PI),
        new(0.25f, 1, 0, 0),
        new(0.5f, 0, -2*Mathf.PI, -2*Mathf.PI),
        new(0.75f, -1, 0, 0),
        new(1, 0, 2*Mathf.PI, 2*Mathf.PI),
    });

    [SerializeField, Tooltip("The duration, in seconds, of our looping bounce animation.\n\n"
                           + "Default: 2")]
    private float fastLoopPeriod = 2;
    [SerializeField, Tooltip("The duration, in seconds, it takes to return to the base state when "
                           + "the animation stops.\n\nDefault: 0.025")]
    private float fastAnimKillTime = 0.025f;

    [Header("Slow Bounce Parameters")]
    [SerializeField, Tooltip("The curve we evaluate when animating the bounce. "
                           + "The duration of the curve is determined by loopPeriod.\n\n"
                           + "Recall negative is squash and positive is stretch.")]
    private AnimationCurve slowBounceCurve = new(new Keyframe[]{
        // These points plot a sine curve.
        new(0, 0, 2*Mathf.PI, 2*Mathf.PI),
        new(0.25f, 1, 0, 0),
        new(0.5f, 0, -2*Mathf.PI, -2*Mathf.PI),
        new(0.75f, -1, 0, 0),
        new(1, 0, 2*Mathf.PI, 2*Mathf.PI),
    });

    [SerializeField, Tooltip("The duration, in seconds, of our looping bounce animation.\n\n"
                           + "Default: 2")]
    private float slowLoopPeriod = 2;
    [SerializeField, Tooltip("The duration, in seconds, it takes to return to the base state when "
                           + "the animation stops.\n\nDefault: 0.025")]
    private float slowAnimKillTime = 0.025f;

    // Start is called before the first frame update
    void Start()
    {
        SquashStretchBouncer = GetComponent<SquashStretchBouncer>();
        if (SquashStretchBouncer == null)
        {
            Debug.LogError(gameObject.name + ": SquetchStarter: cannot find Squash Stretch Bouncer");
        }
    }
    
    /// <summary>
    /// Subscribes the squetch functions to play during dialogue.
    /// </summary>
    public void Subscribe()
    {
        GameManager.DialogueManager.StartFastBounce += StartFast;
        GameManager.DialogueManager.StartSlowBounce += StartSlow;
        GameManager.DialogueManager.StopBounce += StopSquetch;
    }

    void OnDisable()
    {
        GameManager.DialogueManager.StartFastBounce -= StartFast;
        GameManager.DialogueManager.StartSlowBounce -= StartSlow;
        GameManager.DialogueManager.StopBounce -= StopSquetch;
    }

    private void StartFast()
    {
        SquashStretchBouncer.bounceCurve = fastBounceCurve;
        SquashStretchBouncer.loopPeriod = fastLoopPeriod;
        SquashStretchBouncer.animKillTime = fastAnimKillTime;
        SquashStretchBouncer.StartAnim();
    }

    private void StartSlow()
    {
        SquashStretchBouncer.bounceCurve = slowBounceCurve;
        SquashStretchBouncer.loopPeriod = slowLoopPeriod;
        SquashStretchBouncer.animKillTime = slowAnimKillTime;
        SquashStretchBouncer.StartAnim();
    }

    private void StopSquetch()
    {
        SquashStretchBouncer.StopAnim();
    }
}
