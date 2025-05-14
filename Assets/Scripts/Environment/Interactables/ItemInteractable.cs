using UnityEngine;

public class ItemInteractable : Interactable
{
    public override void Interaction() 
    {
        Debug.Log("Interaction called on: " + gameObject.name);
    }

    public override void Highlight() 
    {

    }

    public override void UnHighlight() 
    {

    }
}
