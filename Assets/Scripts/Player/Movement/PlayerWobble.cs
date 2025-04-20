using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerWobble : MonoBehaviour
{
    private enum Facing
    {
        Left, Right
    }

    #region ======== [ PARAMETERS ] ========


    [Header("Object References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Billboard billboard;
    [SerializeField] private Transform model;
    [SerializeField] private Transform leftPivot;
    [SerializeField] private Transform rightPivot;

    [Header("Parameters")]
    [SerializeField] private float walkAmplitude = 5f;
    [SerializeField] private float walkHorizontalSpeed = 17f;
    [SerializeField] private float walkVerticalSpeed = 10f;
    [SerializeField] private float walkStartTransition = 0.25f;
    [SerializeField] private float resetDuration = 0.5f;
    [SerializeField] private Ease resetEase = Ease.OutElastic;

    [SerializeField] private bool snapBeforeStop = false;
    [SerializeField, Range(0, 1)] private float snapFactor = 0.5f;
    [SerializeField] private float lerpSpeed = 10f;

    [Header("3D Walk Parameters")]
    [SerializeField] private bool walk3D;
    [SerializeField] private float walk3DAmplitude;

    [Header("Stop Motion Parameters")]
    [SerializeField] private bool stopMotion;
    [SerializeField] private float frameRate = 12;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private bool _animatingWalk = false;
    private Facing _facing = Facing.Left;
    private Transform _pivot;
    private Vector3 _leftPivotPos;
    private Vector3 _rightPivotPos;
    private Vector3 _targetPos;
    private Quaternion _targetRot = Quaternion.identity;
    private float _walkTimer = 0;
    private float _frameRateTimer = 0;

    #endregion

    #region ======== [ STATE FUNCTIONS ] ========

    /// <summary>
    /// Handles walking animation in Update
    /// </summary>
    private void AnimateWalking()
    {
        // Balance between horizontal and vertical walking speed
        var t = (Mathf.Abs(player.Input.y) - Mathf.Abs(player.Input.x)) / 2f + 0.5f;
        var walkSpeed = Mathf.Lerp(walkHorizontalSpeed, walkVerticalSpeed, t) * player.Input.magnitude;

        _walkTimer += Time.deltaTime * walkSpeed;

        // If stop motion, only animate certain frames
        if (stopMotion)
        {
            _frameRateTimer += Time.deltaTime;

            if (_frameRateTimer < 1f / frameRate)
            {
                return;
            }
            else
            {
                _frameRateTimer = 0;
            }
        }


        float rotY = 0, rotZ = 0;
        var maxAngle = Mathf.Lerp(0, walkAmplitude, _walkTimer / walkStartTransition); // steadily transitions into the walk animation
        rotZ += Mathf.Sin(1.5f * Mathf.Cos(_walkTimer)) * maxAngle; // modified sin function to modulate movement

        SwapPivot(rotZ < 0 ? leftPivot : rightPivot); // swap pivot according to

        // for animating the 3D turning walk
        if (walk3D)
        {
            rotY += Mathf.Sin(1.5f * Mathf.Cos(_walkTimer + Mathf.PI / 2f)) * walk3DAmplitude;
            rotY *= _facing == Facing.Left ? 1 : -1;
            rotY *= player.Input.y < 0 ? 1 : -1;
        }

        // updates the pivot to be correct
        rightPivot.transform.localRotation = Quaternion.Euler(0, rotY, rotZ);
        leftPivot.transform.localRotation = Quaternion.Euler(0, rotY, rotZ);
    }


    /// <summary>
    /// Handles idle animation in Update
    /// </summary>
    private void AnimateIdle()
    {
        // Does nothing right now
    }


    /// <summary>
    /// Runs when the player transitions into a walk
    /// </summary>
    private void StartWalk()
    {
        _pivot.DOKill();
        _walkTimer = 0;
        _frameRateTimer = 0;
    }


    /// <summary>
    /// Runs when the player stops walking
    /// </summary>
    private void EndWalk()
    {
        _pivot.transform.DOKill();
        _targetRot = Quaternion.identity;
        if (snapBeforeStop)
        {
            var snapAngle = Mathf.Sign(transform.localRotation.x) * walkAmplitude * snapFactor;
            _pivot.transform.localRotation = Quaternion.Euler(0, 0, snapAngle);
        }
        
        if (stopMotion)
        {
            _pivot.transform.DOLocalRotate(Vector3.zero, resetDuration).SetEase(EaseFactory.StopMotion(Mathf.RoundToInt(frameRate), resetEase));
        }
        else
        {
            _pivot.transform.DOLocalRotate(Vector3.zero, resetDuration).SetEase(resetEase);
        }
    }

    #endregion

    #region ======== [ UTILITY FUNCTIONS ] ========

    // Swap which side the pivot is on. Will ignore if on the same pivot
    private void SwapPivot(Transform pivot)
    {
        if (_pivot == null) _pivot = pivot;
        if (_pivot == pivot) return;

        model.transform.SetParent(pivot, true);
        _targetPos = pivot == leftPivot ? _leftPivotPos : _rightPivotPos;
        _targetRot = Quaternion.Euler(model.transform.localRotation.x, model.transform.localRotation.y, 0);
        _pivot = pivot;
    }

    /// <summary>
    /// Flips the character to right direction depending on player input
    /// </summary>
    private void HandleFlip()
    {
        if (player.Input.x > 0)
        {
            if (_facing == Facing.Right) return;

            billboard.Flip();
            _facing = Facing.Right;
        }
        else if (player.Input.x < 0)
        {
            if (_facing == Facing.Left) return;

            billboard.Flip();
            _facing = Facing.Left;
        }
    }

    #endregion

    #region ======== [ BUILT-IN UNITY ] ========

    void Update()
    {
        // Handles player animation states
        if (player.IsWalking)
        {
            if (!_animatingWalk)
            {
                StartWalk();
                _animatingWalk = true;
            }
            AnimateWalking();
            HandleFlip();

        }
        else
        {
            if (_animatingWalk)
            {
                EndWalk();
                _animatingWalk = false;
            }
            AnimateIdle();
        }

        // lerping to target to avoid sudden snaps
        model.transform.localPosition = Vector3.Lerp(model.transform.localPosition, _targetPos, Time.deltaTime * lerpSpeed);
        model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, _targetRot, Time.deltaTime * lerpSpeed);
    }

    void Start()
    {
        // Initalize Pivot Positions
        model.transform.SetParent(leftPivot);
        _leftPivotPos = model.transform.localPosition;
        model.transform.SetParent(rightPivot);
        _rightPivotPos = model.transform.localPosition;
        _pivot = rightPivot;
    }


    void Reset()
    {
        var player = transform.parent.GetComponentInParent<PlayerMovement>();
        if (player) this.player = player;
    }

    #endregion
}
