using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInteractable : ItemInteractable
{
    [SerializeField] private ElevatorController controller;
    [SerializeField] private NewElevatorController cont;

    public override void Interaction()
    {
        base.Interaction();
        if (controller)
            controller.MoveElevator();
        else if (cont)
            cont.MoveElevator();
    }
}
