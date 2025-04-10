using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class EffectCardData : ScriptableObject
{
    public string Description;
    
    public List<Action> Actions = new List<Action>();

    public bool DoesActivate(BarteringController barteringController)
    {
        foreach (Action action in Actions)
        {
            if (action.DoesActivate(barteringController))
            {
                return true;
            }
        }

        return false;
    }

    public void Activate(BarteringController barteringController)
    {
        foreach (Action action in Actions)
        {
            if (action.DoesActivate(barteringController))
            {
                action.Activate(barteringController);
            }
        }
    }
}


public interface Action
{
    public bool DoesActivate(BarteringController barteringController);
    public void Activate(BarteringController barteringController);
}


public class MultiplyValueBasedOnTags : Action
{
    // public List<Tag> Tags;
    public float ValueMultiplier = 1f;

    public bool DoesActivate(BarteringController barteringController)
    {
        // for every items in the barter, if it is a 
        return true;
    }

    public void Activate(BarteringController barteringController)
    {

    }
}


public class AddValueBasedOnTags : Action
{
    // public List<Tag> Tags;
    public float ValueAddend = 0f;

    public bool DoesActivate(BarteringController barteringController)
    {
        // for every items in the barter, if it is a 
        return true;
    }

    public void Activate(BarteringController barteringController)
    {

    }
}