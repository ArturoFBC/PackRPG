using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionOnSKillType
{
    APPLIED_TO_ALL_SKILLS,
    HAS_TAG,
    IS_BUFF,
    IS_DEBUFF,
    IS_HEAL,
}

public enum ConditionOnCreatureType
{
    ALWAYS_ACTIVE,
    HAS_MORE_THAN_X_HP,
    HAS_LESS_THAN_X_HP,
    HAS_MORE_THAN_X_MANA,
    HAS_LESS_THAN_X_MANA,
    RECEIVED_CRITICALHIT,
    ARISED,
    SPENT_MANA,
    IS_VIGOROUS_OR_STRONGER
}

[System.Serializable]
public class ConditionOnCreature
{
    public ConditionOnCreatureType type;
    public float floatValue;

    // Check if a creature (target) meets this condition
    public bool DoesCreatureMeetCondition( GameObject target )
    {
        switch (type)
        {
            case ConditionOnCreatureType.ALWAYS_ACTIVE:
                return true;
            case ConditionOnCreatureType.HAS_LESS_THAN_X_HP:
                {
                    CreatureHitPoints hitPoints = target.GetComponent<CreatureHitPoints>();
                    return ((hitPoints.currentHP / hitPoints.MaxHP) < floatValue);
                }
            case ConditionOnCreatureType.HAS_MORE_THAN_X_HP:
                {
                    CreatureHitPoints hitPoints = target.GetComponent<CreatureHitPoints>();
                    return ((hitPoints.currentHP / hitPoints.MaxHP) > floatValue);
                }
            case ConditionOnCreatureType.HAS_LESS_THAN_X_MANA:
                {
                    CreatureMana mana = target.GetComponent<CreatureMana>();
                    return ((mana.currentMP / mana.MaxMP) < floatValue);
                }
            case ConditionOnCreatureType.HAS_MORE_THAN_X_MANA:
                {
                    CreatureMana mana = target.GetComponent<CreatureMana>();
                    return ((mana.currentMP / mana.MaxMP) > floatValue);
                }
        }

        return false;
    }
}

[System.Serializable]
public class ConditionOnSKill
{
    public ConditionOnSKillType type;
    public float floatValue;
    public SkillTag tagValue;

    // Check if a creature (target) meets this condition
    public bool DoesCreatureMeetCondition(BaseSkill skill)
    { 
        switch (type)
        {
            case ConditionOnSKillType.APPLIED_TO_ALL_SKILLS:
                return true;
            case ConditionOnSKillType.HAS_TAG:
                if (skill.tags.Contains(tagValue))
                    return true;
                break;
            case ConditionOnSKillType.IS_BUFF:
                if ((skill.targetType != TargetType.ENEMY) &&
                    (skill.StatModifiersForTarget != null && skill.StatModifiersForTarget.Count > 0) &&
                    (skill.StatModifiersForTarget[0].value > 0))
                {
                    return true;
                }
                break;
            case ConditionOnSKillType.IS_DEBUFF:
                if ((skill.targetType == TargetType.ENEMY || skill.targetType == TargetType.GROUND) &&
                    (skill.StatModifiersForTarget != null && skill.StatModifiersForTarget.Count > 0) &&
                    (skill.StatModifiersForTarget[0].value < 0))
                {
                    return true;
                }
                break;
        }
        return false;
    }
}