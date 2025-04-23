using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using DG.Tweening;
using System;
using UnityEngine.Rendering;

[ExecuteAlways]
public class NewElevatorController : MonoBehaviour
{
    #region ========== [ PARAMETERS ] ==========

    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    [SerializeField] private bool openLeft, openRight, openFront, openBack; // ----- ADDED ------

    [Header("References")]
    [SerializeField] private Transform objectRoot;

    [BoxGroup("Setup"), SerializeField] private InventoryCardData requiredCard;
    [BoxGroup("Setup"), SerializeField] private Transform startingWaypoint; // Contractor note: why not waypoints[0]?
    [BoxGroup("Setup"), SerializeField, Range(0f, 10f)] private float movementDurationSeconds;
    // [BoxGroup("Setup"), SerializeField] private GameObject door; // Contractor note: would not need this because of the fence list

    [SerializeField] private GameObject leftFence, rightFence, frontFence, backFence; // ----- ADDED ------
    [SerializeField] private bool[] openPoints1 = new bool[4];
    [SerializeField] private bool[] openPoints2 = new bool[4];

    [Header("SFX")]
    [SerializeField] private AudioEvent elevatorStartSFX;
    [SerializeField] private AudioEvent elevatorHumSFX;
    #endregion

    #region ========== [ PRIVATE PROPERTIES ] ==========

    private int _startingWaypointIndex = -1;
    private int _nextIndexModifier = 1;
    private int _currentWaypointIndex = 0;
    private bool _moving = false;

    #endregion

    #region ========== [ PUBLIC METHODS ] ==========

    /// <summary>
    /// Moves the elevator along the Waypoints in order, reverses when it reaches the end.
    /// </summary>
    [Button("Move Elevator")]
    public void MoveElevator()
    {
        if (requiredCard != null && !GameManager.Inventory.HasCard(requiredCard)) return;
        if (_moving) { return; }

        // Start playing elevator sfx upon move
        elevatorStartSFX.Play(gameObject);
        elevatorHumSFX.Play(gameObject);

        SetMoving(true);

        if ((_startingWaypointIndex == waypoints.Count - 1 && _nextIndexModifier > 0) ||
            (_startingWaypointIndex == 0 && _nextIndexModifier < 0))
        {
            _nextIndexModifier *= -1;
        }
        int targetWaypointIndex = _startingWaypointIndex + _nextIndexModifier;
        Transform targetWaypoint = waypoints[targetWaypointIndex];

        objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds) // CHANGED
            .SetEase(Ease.InOutQuad).OnComplete(() => {
                SetMoving(false);

            
            });

        startingWaypoint = targetWaypoint;
        SetStartingWaypointIndex();
    }

    /// <summary>
    /// Moves the elevator along the Waypoints in order, to the target waypoint.
    /// </summary>
    public void MoveElevator(Transform targetWaypoint)
    {
        if (requiredCard != null && !GameManager.Inventory.HasCard(requiredCard)) return;
        if (_moving) return;

        // Start playing elevator sfx upon move
        elevatorStartSFX.Play(gameObject);
        elevatorHumSFX.Play(gameObject);

        int targetIndex = GetWaypointIndex(targetWaypoint);

        if (_startingWaypointIndex == targetIndex) return;

        SetMoving(true);

        // Set nextIndexModifier so that we are moving towards the target
        if ((_startingWaypointIndex > targetIndex && _nextIndexModifier > 0) ||
            (_startingWaypointIndex < targetIndex && _nextIndexModifier < 0))
        {
            _nextIndexModifier *= -1;
        }

        // Moving from start to end: InOutQuad, duration is movementDurationSeconds
        if (_startingWaypointIndex == _currentWaypointIndex && NextWaypointIs(targetWaypoint))
        {
            objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds)
                .SetEase(Ease.InOutQuad).OnComplete(() => { SetMoving(false); });
            startingWaypoint = targetWaypoint;
            SetStartingWaypointIndex();
            return;
        }

        // Moving from start to in-between: InQuad, duration is movementDurationSeconds
        if (_startingWaypointIndex == _currentWaypointIndex && !NextWaypointIs(targetWaypoint))
        {
            Transform nextWaypoint = waypoints[_currentWaypointIndex + _nextIndexModifier];

            objectRoot.transform.DOMove(nextWaypoint.position, movementDurationSeconds)
                .SetEase(Ease.InQuad).OnComplete(() => { SetMoving(false); MoveElevator(targetWaypoint); });
            _currentWaypointIndex += _nextIndexModifier;
            return;
        }

        // Moving from in-between to in-between: no ease, duration is a bit shorter
        if (_startingWaypointIndex != _currentWaypointIndex && !NextWaypointIs(targetWaypoint))
        {
            Transform nextWaypoint = waypoints[_currentWaypointIndex + _nextIndexModifier];

            objectRoot.transform.DOMove(nextWaypoint.position, movementDurationSeconds * .75f)
                .SetEase(Ease.Linear).OnComplete(() => { SetMoving(false); MoveElevator(targetWaypoint); });
            _currentWaypointIndex += _nextIndexModifier;
            return;
        }

        // Moving from in-between to end: OutQuad, duration is movementDurationSeconds
        if (_startingWaypointIndex != _currentWaypointIndex && NextWaypointIs(targetWaypoint))
        {
            objectRoot.transform.DOMove(targetWaypoint.position, movementDurationSeconds)
                .SetEase(Ease.OutQuad).OnComplete(() => { SetMoving(false); });
            startingWaypoint = targetWaypoint;
            SetStartingWaypointIndex();
            return;
        }
    }

    /// <summary>
    /// Snaps the position of Object in the elevator to the selected starting waypoint
    /// </summary>
    [SerializeField, Button("Snap Object to Waypoint")]
    public void SnapObjectToWaypoint()
    {
        if (objectRoot != null)
        {
#if UNITY_EDITOR
            Undo.RecordObject(objectRoot, "Move Elevator Object");
#endif

            objectRoot.position = startingWaypoint.position;

#if UNITY_EDITOR
            EditorUtility.SetDirty(objectRoot);
#endif

        }
    }

    /// <summary>
    /// Snaps the position of the selected starting waypoint to the Object in the elevator
    /// </summary>
    public void SnapWaypointToObject()
    {
        if (objectRoot != null)
        {
#if UNITY_EDITOR
            Undo.RecordObject(objectRoot, "Move waypoint");
#endif

            startingWaypoint.position = objectRoot.position;

#if UNITY_EDITOR
            EditorUtility.SetDirty(objectRoot);
#endif

        }
    }

    #endregion

    #region ========== [ PRIVATE METHODS ] ==========

    /// <summary>
    /// Gets the index of targetWaypoint in the waypoints list.
    /// </summary>
    private int GetWaypointIndex(Transform targetWaypoint)
    {
        int returnIndex = -1;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == targetWaypoint)
            {
                returnIndex = i;
                break;
            }
        }
        if (returnIndex == -1)
        {
            Debug.LogError("Elevator: Could not get Waypoint Index, waypoint is not in Waypoints list.");
        }

        return returnIndex;
    }

    /// <summary>
    /// Returns whether or not the next waypoint is the provided waypoint.
    /// </summary>
    private bool NextWaypointIs(Transform waypoint)
    {
        int tempWaypointIndex = _currentWaypointIndex + _nextIndexModifier;
        if ((tempWaypointIndex >= waypoints.Count) ||
            (tempWaypointIndex < 0))
        {
            _nextIndexModifier *= -1;
            tempWaypointIndex = _currentWaypointIndex + _nextIndexModifier;
        }

        return waypoints[tempWaypointIndex] == waypoint;
    }

    /// <summary>
    /// Sets the startingWaypointIndex and currentWaypointIndex variables.
    /// </summary>
    private void SetStartingWaypointIndex()
    {
        if (!startingWaypoint) Debug.LogError("Elevator: Starting Waypoint not Assigned");

        _startingWaypointIndex = GetWaypointIndex(startingWaypoint);
        _currentWaypointIndex = _startingWaypointIndex;
    }

    void OnValidate()
    {
        if (waypoints.Count > 0 && startingWaypoint != null)
        {
            SnapObjectToWaypoint();
        }
    }

    private void SetMoving(bool value)
    {
        _moving = value;
        
        if(transform.position == waypoints[0].position)
        {
            InitializeDoors(openPoints1[0], openPoints1[1], openPoints1[2], openPoints1[3]);
        }
        else if (transform.position == waypoints[1].position)
        {
            InitializeDoors(openPoints2[0], openPoints2[1], openPoints2[2], openPoints2[3]);
        }
        // if (_moving) { door.SetActive(true); }
        // else { door.SetActive(false); }
    }

    /*
     * 
     *       ------- NEW FUNCTIONS -----------
     * 
     */

    private void InitializeDoors()
    {
        leftFence.SetActive(!openLeft);
        rightFence.SetActive(!openRight);
        frontFence.SetActive(!openFront);
        backFence.SetActive(!openBack);
    }

    private void InitializeDoors(bool left, bool right, bool front, bool back)
    {
        leftFence.SetActive(left);
        rightFence.SetActive(right);
        frontFence.SetActive(front);
        backFence.SetActive(back);
    }

    /*
     * 
     *          ---- END NEW FUNCTIONS --------
     * 
     */

    void Start()
    {
        SetStartingWaypointIndex();
        InitializeDoors();
    }

    void Update()
    {
        InitializeDoors();
    }
    #endregion
}
