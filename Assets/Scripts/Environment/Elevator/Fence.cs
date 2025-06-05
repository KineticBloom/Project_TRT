using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    [SerializeField] private ElevatorController elevator;
    [Tooltip("The index of the waypoint this is connected to. This index is found in the Elevator's waypoints list")]
    [SerializeField] private int connectedWaypointIndex = 0;
    private BoxCollider _boxCollider;
    private Animator _animator;


    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _animator = GetComponent<Animator>();
        elevator.finishedMoving += OnFinished;
        OnFinished();
    }

    private void OnFinished()
    {
        if (elevator._currentWaypointIndex == connectedWaypointIndex)
        {
            _boxCollider.enabled = false;
            _animator.SetBool("IsOpen", true);
        } else
        {
            _boxCollider.enabled = true;
            _animator.SetBool("IsOpen", false);
        }
    }
}
