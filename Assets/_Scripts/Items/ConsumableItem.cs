using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    BAIT
}

public abstract class ConsumableItem : Item
{
    static readonly float[] tierStatIncreaseBaseValues = { 10f, 20f, 40f, 65f, 100f };

    public ConsumableItem() : base()
    {

    }

    public ConsumableItem(ItemSaveData saveData) : base(saveData)
    {

    }

    public abstract override bool AreEqual(Item otherItem);

    public override int GetCurrencyValue()
    {
        return (int)tierStatIncreaseBaseValues[tier];
    }

    public override ItemType GetItemType()
    {
        return ItemType.CONSUMABLE;
    }

    public abstract TargetType GetTargetType();

    /// <summary>
    /// USE ITEM NO TARGET
    /// </summary>
    public virtual void UseItem()
    {
        Debug.LogError("This item requires a target or has not action for use without a target");
    }

    public virtual void UseItem(Vector3 targetPosition)
    {
        Debug.LogError("This item can not target ground or has not action for use with a target position");
    }

    public virtual void UseItem( GameObject targetObject )
    {
        Debug.LogError("This item can not target a creature/object, or has not action for use with a target creature/object");
    }
}
