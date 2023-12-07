using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsItemsOnInteract : DropsItems, IInteractable
{
    public void Interact(Transform whoActivatedMe)
    {
        DropItems();
        gameObject.tag = "Untagged";

        Collider myCollider = GetComponent<Collider>();
        if ( myCollider != null )
            myCollider.enabled = false;
    }

    protected override List<Drop> GetDrops()
    {
        return GetItemDrops();
    }
}
