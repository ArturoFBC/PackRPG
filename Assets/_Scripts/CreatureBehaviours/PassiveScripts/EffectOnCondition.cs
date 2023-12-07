using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOnCondition : PassiveSkill
{
    CreatureStats _MyCreatureStats;

    public StatModifier _MyBuff;
    public ConditionOnCreature _MyCondition;

    bool activateWhenGreater = false;

    bool applied = false;

    private void Awake()
    {
        if (_MyCreatureStats == null)
            _MyCreatureStats = GetComponent<CreatureStats>();
    }

    protected override void Set()
    {
        if (_MyBasePassive.ModifiersOnCondition != null && _MyBasePassive.ModifiersOnCondition.Count > 0)
        {
            _MyBuff = _MyBasePassive.ModifiersOnCondition[0].modifier;
            _MyCondition = _MyBasePassive.ModifiersOnCondition[0].condition;
        }

        OnEnable();
    }

    void OnEnable()
    {
        if (_MyBasePassive != null )
        {
            ConditionOnCreature condition = _MyBasePassive.ModifiersOnCondition[0].condition;

            switch (condition.type)
            {
                case ConditionOnCreatureType.ALWAYS_ACTIVE:
                    Activate();
                    break;
                case ConditionOnCreatureType.HAS_LESS_THAN_X_HP:
                    GetComponent<CreatureHitPoints>().HealthChangedEvent += ResourceChanged;
                    activateWhenGreater = false;
                    break;
                case ConditionOnCreatureType.HAS_MORE_THAN_X_HP:
                    GetComponent<CreatureHitPoints>().HealthChangedEvent += ResourceChanged;
                    activateWhenGreater = true;
                    break;
                case ConditionOnCreatureType.HAS_LESS_THAN_X_MANA:
                    GetComponent<CreatureMana>().manaChangedEvent += ResourceChanged;
                    activateWhenGreater = false;
                    break;
                case ConditionOnCreatureType.HAS_MORE_THAN_X_MANA:
                    GetComponent<CreatureMana>().manaChangedEvent += ResourceChanged;
                    activateWhenGreater = true;
                    break;
                case ConditionOnCreatureType.ARISED:
                    GetComponent<CreatureHitPoints>().AriseEvent += EventHappened;
                    break;
            }
        }
    }

    void OnDisable()
    {
        if (_MyBasePassive != null)
        {
            ConditionOnCreature condition = _MyBasePassive.ModifiersOnCondition[0].condition;

            switch (condition.type)
            {
                case ConditionOnCreatureType.HAS_LESS_THAN_X_HP:
                    GetComponent<CreatureHitPoints>().HealthChangedEvent -= ResourceChanged;
                    break;
                case ConditionOnCreatureType.HAS_MORE_THAN_X_HP:
                    GetComponent<CreatureHitPoints>().HealthChangedEvent -= ResourceChanged;
                    break;
                case ConditionOnCreatureType.HAS_LESS_THAN_X_MANA:
                    GetComponent<CreatureMana>().manaChangedEvent -= ResourceChanged;
                    break;
                case ConditionOnCreatureType.HAS_MORE_THAN_X_MANA:
                    GetComponent<CreatureMana>().manaChangedEvent -= ResourceChanged;
                    break;
            }

            Deactivate();
        }
    }

    public void EventHappened()
    {
        Activate();
    }

    public void ResourceChanged(float value, float maxValue)
    {
        float percentage = value / maxValue;

        if ( ( ( percentage > _MyCondition.floatValue ) == activateWhenGreater ) )
            Activate();
        else if ( ( ( percentage > _MyCondition.floatValue) != activateWhenGreater ))
            Deactivate();
    }

    void Activate()
    {
        if (_MyBuff != null)
            _MyCreatureStats.AddBuff(_MyBuff);
    }

    void Deactivate()
    {
        if (_MyBuff != null)
            _MyCreatureStats.RemoveBuff(_MyBuff);
    }
}

