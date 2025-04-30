using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{
    [SerializeField] private ElevatorController elevator;
    [Tooltip("The index of the waypoint this is connected to. This index is found in the Elevator's waypoints list")]
    [SerializeField] private int connectedWaypointIndex = 0;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        elevator.finishedMoving += OnFinished;
    }

    private void OnFinished()
    {
        if ( elevator._currentWaypointIndex == connectedWaypointIndex)
        {
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
        } else
        {
            boxCollider.enabled = true;
            meshRenderer.enabled = true;
        }
    }
}
