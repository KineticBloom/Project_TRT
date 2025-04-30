using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInteractable : ItemInteractable
{
    [SerializeField] private ElevatorController controller;
    private Vector3 _savedPos;

    private void Start()
    {
        if (IconLocalPosition != null)
        {
            _savedPos = IconLocalPosition;
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        controller.MoveToOtherEnd();

        // Hide Icon and reveal it when it's done
        if (IconLocalPosition != null)
        {
            UseTransform = false;
            IconLocalPosition = Vector3.down * 100;

            // Turning off the icon if it exists in scene (attached to player)
            InteractionIcon interactionIcon = FindFirstObjectByType<InteractionIcon>();
            if (interactionIcon != null)
            {
                interactionIcon.Hide();
            }

        }
        controller.finishedMoving += OnFinished;
    }

    private void OnFinished()
    {
        // put the interact icon back where it was
        if (IconLocalPosition != null)
        {
            UseTransform = true;
            IconLocalPosition = _savedPos;
        }
        controller.finishedMoving -= OnFinished;
    }
}
