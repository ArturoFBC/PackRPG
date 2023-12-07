using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemEffectImplementation
{
    StatBonus
}

[CreateAssetMenu( menuName = "Items/Equipable")]
public class BaseEquipmentItem : BaseItem
{
    public ItemEffectImplementation skillImplementation;

    public EquipmentType type;

    [Header("Item Effects")]
    public StatModifier[] StatModifiers;
    public StatusEffect[] StatusEffects;
    public DamageOverTime[] DamageOverTimeEffects;

    public override ItemType GetItemType()
    {
        return ItemType.EQUIPMENT;
    }
}
