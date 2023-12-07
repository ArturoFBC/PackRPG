using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    ENEMY,
    ALLY,
    SELF,
    GROUND,
    NONE
}

public enum StatusEffectType
{
    BURNT,
    FROZEN,
    ELECTRIFIED,
    POISON,
    BLEEDING,
    STUN,
    END
}

[System.Serializable]
public abstract class SecondaryEffect
{
    public TargetType target;
    public float duration;
}
[System.Serializable]
public class StatusEffect: SecondaryEffect
{
    public StatusEffectType status;
}
[System.Serializable]
public class DamageOverTime: SecondaryEffect
{
    public ScriptableObject skillRefference;
    public DamageInstance damageInstance;
    public bool timed = true;
    public float timeRemaining = 5f;
    public int stacks = 1;
    public int maxStacks = 1;
    public Sprite icon;

    public DamageOverTime(DamageOverTime original)
    {
        skillRefference = original.skillRefference;
        damageInstance  = original.damageInstance;
        timed           = original.timed;
        timeRemaining   = original.timeRemaining;
        stacks          = original.stacks;
        maxStacks       = original.maxStacks;
        icon            = original.icon;
    }
}

public enum SkillCategory
{
    BASIC,
    COOLDOWN,
    ULTIMATE
}

public enum SkillImplementation
{
    CircleAreaAttack,
    ConeAttack,
    Heal,
    AreaOfEffectDamageOverTime,
    MeleeSingleTarget,
    ProjectileLaunch,
    Provoke,
    ProximityBuff,
    Bolt,
    GroupHealing,
    END
}

[CreateAssetMenu(menuName = "Skills/BaseActiveSkill")]
public class BaseActiveSkill : BaseSkill
{
    public SkillImplementation skillImplementation; 
    
    public SkillCategory category;
    public string animationTrigger;

    //Timings
    public float executionDuration;
    //Time % of the total execution duration at wich the hit applied
    [Range(0, 1)]
    public float hitInstant;
    public float creationsDuration;

    public float manaCost;

    //Sounds
    public AudioClip castingSound;

    // GENERATE NEW SKILL INFO BY APPLYING THE MODIFIERS TO THE BASE SKILL
    public void Initialize(BaseActiveSkill baseSkill, List<StatModifier> modifiers, List<SkillTag> expandedTags)
    {
        skillImplementation = baseSkill.skillImplementation;

        category = baseSkill.category;
        icon = baseSkill.icon;
        projectile = baseSkill.projectile;
        useVFX = baseSkill.useVFX;
        targetVFX = baseSkill.targetVFX;
        animationTrigger = baseSkill.animationTrigger;

        targetType = baseSkill.targetType;

        useRange = baseSkill.useRange;
        effectRange = baseSkill.effectRange;

        damage = baseSkill.damage;
        criticalChance = baseSkill.criticalChance;
        criticalDamage = baseSkill.criticalDamage;
        damageType = baseSkill.damageType;
        damageClass = baseSkill.damageClass;
        lifeSteal = baseSkill.lifeSteal;

        executionDuration = baseSkill.executionDuration;

        hitInstant = baseSkill.hitInstant;
        cooldown = baseSkill.cooldown;
        creationsDuration = baseSkill.creationsDuration;

        manaCost = baseSkill.manaCost;

        StatModifiersForTarget = new List<StatModifier>( baseSkill.StatModifiersForTarget );
        StatusEffects = new List<StatusEffect>(baseSkill.StatusEffects);
        DamageOverTimeEffects = new List<DamageOverTime>( baseSkill.DamageOverTimeEffects );
        ModifiersOnCondition = new List<ModifierOnCondition>( baseSkill.ModifiersOnCondition );

        tags = expandedTags;

        Dictionary<BuffType, float> multiplicativeAccumulatedModifier = new Dictionary<BuffType, float>();
        Dictionary<BuffType, float> additiveAccumulatedModifier       = new Dictionary<BuffType, float>();

        for (int i = 0; i < (int)BuffType.END; i++)
        {
            multiplicativeAccumulatedModifier.Add((BuffType)i, 0);
            additiveAccumulatedModifier.Add((BuffType)i, 0);
        }

        // APPLY MODIFIERS
        foreach (StatModifier modifier in modifiers)
        {
            if (modifier.skillCondition == null || modifier.skillCondition.DoesCreatureMeetCondition(this) )
            {
                if (modifier.targetCondition == null || modifier.targetCondition.type == ConditionOnCreatureType.ALWAYS_ACTIVE)
                { 
                    switch (modifier.operation)
                    {
                        case StatModifier.ModifierOperation.ADDITIVE:
                            additiveAccumulatedModifier[modifier.stat] += modifier.value;
                            break;
                        case StatModifier.ModifierOperation.MULTIPLICATIVE:
                            multiplicativeAccumulatedModifier[modifier.stat] += modifier.value;
                            break;
                    }
                }
                else
                {
                    ModifierOnCondition modifierOnCondition = new ModifierOnCondition();
                    modifierOnCondition.condition = modifier.targetCondition;
                    modifierOnCondition.modifier = modifier;
                    ModifiersOnCondition.Add(modifierOnCondition);
                }
            }
        }

        for (int i = 0; i < (int)BuffType.END; i++)
        {
            switch ((BuffType)i)
            {
                case BuffType.DAMAGE:
                    damage += additiveAccumulatedModifier[(BuffType)i];
                    damage *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    break;
                case BuffType.CRIT_BONUS:
                    criticalChance += additiveAccumulatedModifier[(BuffType)i];
                    criticalChance *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    break;
                case BuffType.CRIT_DAMAGE:
                    criticalDamage += additiveAccumulatedModifier[(BuffType)i];
                    criticalDamage *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    break;
                case BuffType.COOLDOWN:
                    cooldown += additiveAccumulatedModifier[(BuffType)i];
                    cooldown *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    break;
                case BuffType.ATTACK_DURATION:
                    executionDuration += additiveAccumulatedModifier[(BuffType)i];
                    executionDuration *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    break;
                case BuffType.EFFECT_DURATION:
                    foreach (StatModifier mod in StatModifiersForTarget)
                    {
                        mod.duration += additiveAccumulatedModifier[(BuffType)i];
                        mod.duration *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    }
                    break;
                case BuffType.INFLICTED_STATUS_DURATION:
                    foreach (StatusEffect status in StatusEffects)
                    {
                        status.duration += additiveAccumulatedModifier[(BuffType)i];
                        status.duration *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    }
                    break;
                case BuffType.DOT_DAMAGE:
                    foreach (DamageOverTime dot in DamageOverTimeEffects)
                    {
                        dot.damageInstance.damage += additiveAccumulatedModifier[(BuffType)i];
                        dot.damageInstance.damage *= 1 + multiplicativeAccumulatedModifier[(BuffType)i];
                    }
                    break;
            }
        }
    }
}
