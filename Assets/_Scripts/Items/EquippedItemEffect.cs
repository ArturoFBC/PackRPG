using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquippedItemEffect : MonoBehaviour
{
    [SerializeField] protected Item myItem;
    [SerializeField] protected CreatureStats myCreatureStats;

    private void Awake()
    {
        myCreatureStats = GetComponent<CreatureStats>();
    }

    public abstract void OnEquip();
    public abstract void OnUnequip();

    static public EquippedItemEffect AssignToCreature(GameObject creature, Item item)
    {
        EquippedItemEffect newEquippedItemEffect = null;
        if (item is StatModifierItem)
        {
            newEquippedItemEffect = creature.AddComponent<EquippedStatModifierEffect>() as EquippedItemEffect;
            newEquippedItemEffect.myItem = item;
            newEquippedItemEffect.OnEquip();
        }
        else
        {

        }

        return newEquippedItemEffect;
    }
}

public class EquippedStatModifierEffect : EquippedItemEffect
{
    [SerializeField] private List<StatModifier> myModifiers;

    private void OnEnable()
    {
        OnEquip();
    }

    private void OnDisable()
    {
        OnUnequip();
    }

    public override void OnEquip()
    {
        if (myItem == null || ((myItem is StatModifierItem) == false))
            return;
    }

    public override void OnUnequip()
    {
        if (myModifiers != null && myModifiers.Count > 0)
        {
            foreach (StatModifier modifier in myModifiers)
                myCreatureStats.RemoveBuff(modifier);
        }
    }
}