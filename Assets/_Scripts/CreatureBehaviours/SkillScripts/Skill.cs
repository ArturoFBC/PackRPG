using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum DamageType
{
    NONE,
    PIERCE,
    SLASH,
    BLUNT,
    FIRE,
    ICE,
    LIGHTING,
    POISION,
    BLEEDING
}

public enum DamageClass
{
    NONE,
    PHYSICAL,
    ETHEREAL
}

[System.Serializable]
public struct DamageInstance
{
    public float damage;
    public DamageType type;
    public DamageClass damageClass;
}

public struct SkillHit
{
    public DamageInstance[] damageInstances;
    public float strength;
    public float intelligence;
    public float baseCritChance;
    public float dextrity;
    public float bonusCritDamage;
    public List<StatModifier> statModifiers;
    public List<StatusEffect> statusEffects;
    public List<DamageOverTime> damageOverTimeEffects;
    public List<ModifierOnCondition> targetBasedModifiers;
    public GameObject attacker;
    public Vector3 attackPosition;
    public float lifeSteal;

    public void ApplyTargetModifiers(GameObject target)
    {
        if (targetBasedModifiers != null && targetBasedModifiers.Count > 0)
        {
            foreach (ModifierOnCondition modifierOnCondition in targetBasedModifiers)
            {
                if ( modifierOnCondition.condition.DoesCreatureMeetCondition( target ) )
                {
                    switch (modifierOnCondition.modifier.stat)
                    {
                        case BuffType.DAMAGE:
                            if (damageInstances != null && damageInstances.Length > 0) damageInstances[0].damage *= 1 + modifierOnCondition.modifier.value;
                            break;
                        case BuffType.CRIT_BONUS:
                            baseCritChance *= 1 + modifierOnCondition.modifier.value;
                            break;
                        case BuffType.CRIT_DAMAGE:
                            bonusCritDamage += modifierOnCondition.modifier.value;
                            break;
                        case BuffType.EFFECT_DURATION:
                            foreach (StatModifier mod in statModifiers)
                                mod.duration *= 1 + modifierOnCondition.modifier.value;
                            break;
                        case BuffType.INFLICTED_STATUS_DURATION:
                            foreach (StatusEffect status in statusEffects)
                                status.duration *= 1 + modifierOnCondition.modifier.value;
                            break;
                        case BuffType.DOT_DAMAGE:
                            foreach (DamageOverTime dot in damageOverTimeEffects)
                                dot.damageInstance.damage *= 1 + modifierOnCondition.modifier.value;
                            break;
                    }
                }
            }
        }
    }
}

public struct HealHit
{
    public float baseHeal;
    public float terapeucity;
    public List<StatModifier> statModifiers;
    public List<StatusEffect> statusEffects;
    public List<DamageOverTime> damageOverTimeEffects;
    public List<ModifierOnCondition> targetBasedModifiers;
    public List<SkillTag> tags;
    public GameObject healer;
}

public abstract class Skill : MonoBehaviour {

    private BaseActiveSkill _MyBaseSkill;
    public BaseActiveSkill _CurrentSkillInfo;

    public CreatureStats _MyStats;
    public Animator _MyAnimator;
    public CreatureIABasic _MyCreatureIA;
    public CreatureMana _MyMana;

    public List<SkillTag> _Tags;

    public float cooldownCounter;
    public float executionCounter;

    public bool hasHitYet = false;
    public GameObject targetCreature;

    public delegate void CooldownChanged(float cooldown);
    public event CooldownChanged CooldownChangedEvent;

    public delegate void ExecutionStarted();
    public event ExecutionStarted ExecutionStartedEvent;

    public delegate void ExecutionEnded();
    public event ExecutionEnded ExecutionEndedEvent;

    public bool _IsDestroyed = false;

    public bool isUsable
    {
        get
        {
            return cooldownCounter <= 0 && executionCounter <= 0 && (_MyMana == null || _MyMana.currentMP >= _CurrentSkillInfo.manaCost );
        }
    }
    public TargetType targetType
    {
        get
        {
            return _CurrentSkillInfo.targetType;
        }
    }
    public SkillCategory category
    {
        get
        {
            return _CurrentSkillInfo.category;
        }
    }

    void Start()
    {
        if (_MyStats == null)
            _MyStats = GetComponent<CreatureStats>();
        _MyStats.AddedBuffEvent -= ApplySkillModifiers;
        _MyStats.RemovedBuffEvent -= ApplySkillModifiers;
        _MyStats.AddedBuffEvent += ApplySkillModifiers;
        _MyStats.RemovedBuffEvent += ApplySkillModifiers;
        ApplySkillModifiers(null);

        if (_MyAnimator == null)
            _MyAnimator = GetComponentInChildren<Animator>();

        if (_MyCreatureIA == null)
            _MyCreatureIA = GetComponent<CreatureIABasic>();

        if (_MyMana == null)
            _MyMana = GetComponent<CreatureMana>();
    }

    void Update()
    {
        if ( executionCounter > 0 )
        {
            if (hasHitYet == false)
            {
                float duration = GetActualExecutionDuration();
                float percentOfTheExecutionTimeThatHasPassed = (duration - executionCounter) / duration;
                if ( percentOfTheExecutionTimeThatHasPassed > _CurrentSkillInfo.hitInstant )
                {
                    UseSkill();
                    hasHitYet = true;
                    _MyCreatureIA.SkillEnded();
                }
            }
            executionCounter -= Time.deltaTime;
            if ( executionCounter <= 0 )
            {
                executionCounter = 0;
                hasHitYet = false;
                ExecutionEndedEvent?.Invoke();
                //finished execution
            }
        }
        else if (cooldownCounter > 0)
        {
            cooldownCounter -= Time.deltaTime;
            CooldownChangedEvent?.Invoke(1 - (cooldownCounter / _CurrentSkillInfo.cooldown));
            if (cooldownCounter <= 0)
            {
                cooldownCounter = 0;
                //finished cooldown
            }
        }
    }

    public bool IsWithinRange(GameObject targetCreature)
    {
        return IsWithinRange(targetCreature.transform.position);
    }

    public bool IsWithinRange(Vector3 targetArea)
    {
        //Everything is squared because it is cheaper resource-wise than calculating a squareroot
        try
        {
            return (targetArea - transform.position).sqrMagnitude < Mathf.Pow(_CurrentSkillInfo.useRange, 2);
        }
        catch (Exception e)
        {
            print(this.name);
        }
        return false;
    }

    protected void ExecutionCounters()
    {
        cooldownCounter = GetActualCooldown();
        executionCounter = GetActualExecutionDuration();
        ExecutionStartedEvent?.Invoke();
    }

    float GetActualCooldown()
    {
        return _CurrentSkillInfo.cooldown / ( 1 + (_MyStats.GetSecondaryStat( BuffType.COOLDOWN ) / 1000 ) );
    }

    float GetActualExecutionDuration()
    {
        return _CurrentSkillInfo.executionDuration / (1 + (_MyStats.GetSecondaryStat(BuffType.ATTACK_DURATION) / 1000));
    }

    protected SkillHit GetSkillHit()
    {
        SkillHit hit = new SkillHit();

        hit.damageInstances = new DamageInstance[1];
        hit.damageInstances[0].damage = _CurrentSkillInfo.damage;
        hit.damageInstances[0].type = _CurrentSkillInfo.damageType;
        hit.damageInstances[0].damageClass = _CurrentSkillInfo.damageClass;

        hit.strength     = _MyStats.GetStat(PrimaryStat.STR);
        hit.intelligence = _MyStats.GetStat(PrimaryStat.INT);

        hit.baseCritChance = _CurrentSkillInfo.criticalChance;
        hit.dextrity = _MyStats.GetStat(PrimaryStat.DEX);

        hit.lifeSteal = _CurrentSkillInfo.lifeSteal;

        //Get all the effects of the skill
        hit.statModifiers = new List<StatModifier>( _CurrentSkillInfo.StatModifiersForTarget );
        hit.statusEffects = new List<StatusEffect>( _CurrentSkillInfo.StatusEffects);
        hit.damageOverTimeEffects = new List<DamageOverTime>(_CurrentSkillInfo.DamageOverTimeEffects);
        hit.targetBasedModifiers = new List<ModifierOnCondition>(_CurrentSkillInfo.ModifiersOnCondition);

        hit.attacker = this.gameObject;
        hit.attackPosition = transform.position;

        return hit;
    }

    protected HealHit GetHealInstance()
    {
        HealHit heal = new HealHit();

        heal.baseHeal = _CurrentSkillInfo.damage;

        heal.terapeucity = RolePlayingFormulas.TerapeucityFromWill(_MyStats.GetStat(PrimaryStat.WILL));

        //Get all the effects of the skill
        heal.statModifiers = new List<StatModifier>(_CurrentSkillInfo.StatModifiersForTarget);
        heal.statusEffects = new List<StatusEffect>(_CurrentSkillInfo.StatusEffects);
        heal.damageOverTimeEffects = new List<DamageOverTime>(_CurrentSkillInfo.DamageOverTimeEffects);
        heal.targetBasedModifiers = new List<ModifierOnCondition>(_CurrentSkillInfo.ModifiersOnCondition);

        heal.healer = this.gameObject;

        return heal;
    }

    protected List<SkillTag> GetSkillTags()
    {
        List<SkillTag> tags = new List<SkillTag>(_MyBaseSkill.tags);

        SkillTag possibleTag;

        // TAG FOR DAMAGE TYPE
        string damageType = _MyBaseSkill.damageType.ToString();
        if (Enum.TryParse(damageType, out possibleTag) && tags.Contains(possibleTag) == false)
            tags.Add(possibleTag);

        // TAG FOR DAMAGE CLASS
        string damageClass = _MyBaseSkill.damageClass.ToString();
        if (Enum.TryParse(damageClass, out possibleTag) && tags.Contains(possibleTag) == false)
            tags.Add(possibleTag);

        // TAG FOR SKILLTYPE CLASS
        string category = _MyBaseSkill.category.ToString();
        if (Enum.TryParse(category, out possibleTag) && tags.Contains(possibleTag) == false)
            tags.Add(possibleTag);

        return tags;
    }

    protected void ApplySkillModifiers( StatModifier modifier )
    {
        List<StatModifier> modifiers = GetComponent<CreatureStats>().GetStatModifiers();
        _CurrentSkillInfo = ScriptableObject.CreateInstance<BaseActiveSkill>();
        _CurrentSkillInfo.Initialize( _MyBaseSkill, modifiers, GetSkillTags() );
    }

    protected bool ValidTarget()
    {
        return targetCreature != null && targetCreature.activeInHierarchy;
    }

    public abstract void StartExecution();

    public abstract void StartExecution(Vector3 targetArea);

    public abstract void StartExecution(GameObject target);

    public abstract void Execute();

    public void UseSkill()
    {
        if (_CurrentSkillInfo.castingSound != null)
            GetComponent<CreatureSound>()?.PlayOneShot(_CurrentSkillInfo.castingSound);

        Execute();
    }

    public virtual GameObject GetTargets()
    {
        return null;
    }

    public void SetDestroyed()
    {
        _MyStats.AddedBuffEvent -= ApplySkillModifiers;
        _MyStats.RemovedBuffEvent -= ApplySkillModifiers;
        _IsDestroyed = true;
    }

    public Sprite GetIcon()
    {
        return _MyBaseSkill.icon;
    }

    static public Skill AssignToCreature(GameObject creature, BaseActiveSkill baseSkill)
    {
        Skill newSkill;
        switch (baseSkill.skillImplementation)
        {
            case SkillImplementation.AreaOfEffectDamageOverTime:
                newSkill = creature.AddComponent<AreaOfEffectDamageOverTime>() as Skill;
                break;
            case SkillImplementation.CircleAreaAttack:
                newSkill = creature.AddComponent<CircleAreaAttack>() as Skill;
                break;
            case SkillImplementation.ConeAttack:
                newSkill = creature.AddComponent<ConeAttack>() as Skill;
                break;
            case SkillImplementation.Heal:
                newSkill = creature.AddComponent<Heal>() as Skill;
                break;
            case SkillImplementation.ProjectileLaunch:
                newSkill = creature.AddComponent<ProjectileLaunch>() as Skill;
                break;
            case SkillImplementation.Provoke:
                newSkill = creature.AddComponent<Provoke>() as Skill;
                break;
            case SkillImplementation.ProximityBuff:
                newSkill = creature.AddComponent<ProximityBuff>() as Skill;
                break;
            case SkillImplementation.Bolt:
                newSkill = creature.AddComponent<Bolt>() as Skill;
                break;
            case SkillImplementation.GroupHealing:
                newSkill = creature.AddComponent<GroupHealing>() as Skill;
                break;
            default: //MeleeSingleTarget
                newSkill = creature.AddComponent<MeleeSingleTarget>() as Skill;
                break;
        }
        newSkill._MyBaseSkill = baseSkill;
        newSkill._CurrentSkillInfo = ScriptableObject.CreateInstance<BaseActiveSkill>();
        newSkill._CurrentSkillInfo.Initialize(baseSkill, new List<StatModifier>(), newSkill.GetSkillTags());

        newSkill._Tags = newSkill.GetSkillTags();

        return newSkill;
    }
}
