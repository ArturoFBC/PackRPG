using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumable")]
public class BaseConsumableItem : BaseItem
{
    public override ItemType GetItemType()
    {
        return ItemType.CONSUMABLE;
    }
}
