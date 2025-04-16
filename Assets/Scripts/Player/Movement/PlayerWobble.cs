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


    [Header("Object References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private Billboard billboard;

    [Header("Parameters")]
    [SerializeField] private float walkAmplitude = 5f;
    [SerializeField] private float walkPeriod = 17f;
    [SerializeField] private float walkStartTransition = 0.25f;
    [SerializeField] private float resetDuration = 0.5f;
    [SerializeField] private Ease resetEase = Ease.OutElastic;

    [SerializeField] private bool snapBeforeStop = false;
    [SerializeField, Range(0, 1)] private float snapFactor = 0.5f;

    [Header("3D Walk Parameters")]
    [SerializeField] private bool walk3D;
    [SerializeField] private float walk3DAmplitude;

    [Header("Stop Motion Parameters")]
    [SerializeField] private bool stopMotion;
    [SerializeField] private float frameRate = 12;


    private bool _animatingWalk = false;
    private Facing _facing = Facing.Left;
    private float _walkTimer = 0;
    private float _frameRateTimer = 0;

    // Update is called once per frame
    void Update()
    {
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

        Debug.Log(_animatingWalk);
    }


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


    private void AnimateWalking()
    {
        _walkTimer += Time.deltaTime;

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

        var maxAngle = Mathf.Lerp(0, walkAmplitude, _walkTimer / walkStartTransition);
        rotZ = Mathf.Sin(1.5f * Mathf.Cos(_walkTimer * walkPeriod)) * maxAngle;

        if (walk3D)
        {
            rotY = Mathf.Sin(1.5f * Mathf.Sin(_walkTimer * walkPeriod)) * walk3DAmplitude;
        }

        transform.localRotation = Quaternion.Euler(0, rotY, rotZ);
    }

    private void AnimateIdle()
    {

    }

    private void StartWalk()
    {
        transform.DOKill();
        _walkTimer = 0;
        _frameRateTimer = 0;
    }


    private void EndWalk()
    {
        transform.DOKill();
        if (snapBeforeStop)
        {
            var snapAngle = Mathf.Sign(transform.localRotation.x) * walkAmplitude * snapFactor;
            transform.localRotation = Quaternion.Euler(0, 0, snapAngle);
        }
        
        if (stopMotion)
        {
            transform.DOLocalRotate(Vector3.zero, resetDuration).SetEase(EaseFactory.StopMotion(Mathf.RoundToInt(frameRate), resetEase));
        }
        else
        {
            transform.DOLocalRotate(Vector3.zero, resetDuration).SetEase(resetEase);
        }
    }


    void Reset()
    {
        var player = transform.parent.GetComponentInParent<PlayerMovement>();
        if (player) this.player = player;
    }
}
