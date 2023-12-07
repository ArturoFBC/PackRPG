using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : MonoBehaviour
{
    public BasePassiveSkill _MyBasePassive;
    public BasePassiveSkill BasePassive
    {
        set
        {
            _MyBasePassive = value;
            Set();
        }
    }

    public bool _IsDestroyed = false;

    // [_MyBasePassive] is often set after the awake, start and first onenable, so no references to that property can be included in those methods. 
    // Instead, this function is called right after setting up the property so it can be used right away
    protected abstract void Set();

    private void Start()
    {
        CreatureHitPoints creatureLife = GetComponent<CreatureHitPoints>();
        if (creatureLife != null)
        {
            creatureLife.KnockOutEvent  -= OnKnockedOut;
            creatureLife.KnockOutEvent  += OnKnockedOut;
            creatureLife.AriseEvent     -= OnArise;
            creatureLife.AriseEvent     += OnArise;
        }
    }

    void OnKnockedOut()
    {
        enabled = false;
    }

    void OnArise()
    {
        enabled = true;
    }

    static public PassiveSkill AssignToCreature(GameObject creature, BasePassiveSkill baseSkill)
    {
        PassiveSkill newSkill;
        switch (baseSkill.impelementation)
        {
            case PassiveImplementation.PassiveAreaDamage:
                newSkill = creature.AddComponent<PassiveAreaDamage>() as PassiveSkill;
                break;
            case PassiveImplementation.PassiveAreaHealing:
                newSkill = creature.AddComponent<PassiveAreaHealing>() as PassiveSkill;
                break;
            default: //RaiseStatOnCondition
                newSkill = creature.AddComponent<EffectOnCondition>() as PassiveSkill;
                break;
        }
        newSkill.BasePassive = baseSkill;

        return newSkill;
    }

    public void SetDestroyed()
    {
        _IsDestroyed = true;
    }
}
