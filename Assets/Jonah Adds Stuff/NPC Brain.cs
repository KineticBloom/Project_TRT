using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBrain : Interactable {

    public NPCData DataOfThisNPC;
    public InventoryCardData ItemForOffer;

    public override void Interaction() {
        throw new System.NotImplementedException();
    }

    public override void Highlight() {
        // TODO: Add Highlight Shader
    }
    public override void UnHighlight() {
        // TODO: Add Highlight Shader
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + IconLocalPosition, 0.25f);
    }
}
