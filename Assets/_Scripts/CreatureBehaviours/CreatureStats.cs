using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PrimaryStat
{
    HP,
    STR,
    DEF,
    INT,
    WILL,
    DEX,
    AGI,
    MP,
    SLASH_RES,
    PIERCE_RES,
    BLUNT_RES,
    FIRE_RES,
    ICE_RES,
    ELECTRIC_RES,
    POISION_RES,
    BLEED_RES,
    MOVE_SPEED,
    ANIMATION_SPEED,
    END
}

public enum BuffType
{
    //Primary
    HP,
    STR,
    DEF,
    INT,
    WILL,
    DEX,
    AGI,
    MP,

    //Elemental resistances
    SLASH_RES,
    PIERCE_RES,
    BLUNT_RES,
    FIRE_RES,
    ICE_RES,
    ELECTRIC_RES,
    POISION_RES,
    BLEED_RES,

    //Speeds
    MOVE_SPEED,
    ANIMATION_SPEED,
    COOLDOWN,
    ATTACK_DURATION,

    EFFECT_DURATION,
    INFLICTED_STATUS_DURATION,

    //Damages
    DAMAGE,
    DOT_DAMAGE,

    //Critical
    CRIT_BONUS,
    CRIT_DEF,
    CRIT_DAMAGE,

    //Healing
    HEALING,
    HEALING_RECEIVED,

    END
}

[System.Serializable]
public class ModifierOnCondition
{
    public ConditionOnCreature condition;
    public StatModifier modifier;
}

[System.Serializable]
public class StatModifier : SecondaryEffect
{
    public enum ModifierOperation
    {
        MULTIPLICATIVE,
        ADDITIVE
    }

    public BaseSkill skillRefference;
    public BuffType stat;

    [Range(-2f,2f)]
    public float value;
    public ModifierOperation operation = ModifierOperation.MULTIPLICATIVE;
    public bool timed = true;
    public float timeRemaining = 5f;
    public int stacks = 1;
    public int maxStacks = 1;
    public Sprite icon;

    public ConditionOnSKill skillCondition;
    public ConditionOnCreature targetCondition;

    public StatModifier()
    {
    }

    public StatModifier (StatModifier original)
    {
        skillRefference = original.skillRefference;
        stat            = original.stat;
        value           = original.value;
        operation       = original.operation;
        timed           = original.timed;
        timeRemaining   = original.timeRemaining;
        stacks          = original.stacks;
        maxStacks       = original.maxStacks;
        icon            = original.icon;

        skillCondition  = original.skillCondition;
        targetCondition = original.targetCondition;
    }
}

public struct Status
{
    public bool active;
    public float timeRemaining;
}

public delegate void EventDelegate(float statValue);

public class StatEvent
{
    protected event EventDelegate eventdelegate;

    public void Dispatch(float statValue)
    {
        eventdelegate?.Invoke(statValue);
    }

    public static StatEvent operator +(StatEvent statEvent, EventDelegate eventDelegate)
    {
        if (statEvent == null)
            statEvent = new StatEvent();

        statEvent.eventdelegate += eventDelegate;
        return statEvent;
    }

    public static StatEvent operator -(StatEvent statEvent, EventDelegate eventDelegate)
    {
        statEvent.eventdelegate -= eventDelegate;
        return statEvent;
    }
}

public class CreatureStats : MonoBehaviour
{
    /// <summary>
    /// true = player creature / false = enemy creature
    /// </summary>
    public bool _IsPlayer;

    public EnemyTier _MyTier;
    private Specimen _MySpecimen;

    [SerializeField]
    List<StatModifier> _Modifiers = new List<StatModifier>();
    Status[] _StatusEffects = new Status[(int)StatusEffectType.END];

    public delegate void AddedBuff(StatModifier buff);
    public event AddedBuff AddedBuffEvent;
    public delegate void RemovedBuff(StatModifier buff);
    public event RemovedBuff RemovedBuffEvent;

    /// <summary>
    /// Events used when stats are updated
    /// </summary>
    public StatEvent[] _StatEvents = new StatEvent[(int)PrimaryStat.END];

    [CreatureStats]
    public float[] baseStats = new float[(int)PrimaryStat.END];


    private void Awake()
    {
        _IsPlayer = CompareTag("Player");
    }

    public Specimen GetSpecimen()
    {
        return _MySpecimen;
    }

    public void SetSpecimen( Specimen specimen, EnemyTier tier = EnemyTier.STANDARD )
    {
        _MySpecimen = specimen;
        _MyTier = tier;

        Level myLevel = GetComponent<Level>();

        myLevel.levelUpEvent -= UpdateStats;
        myLevel.levelUpEvent += UpdateStats;

        UpdateStats(myLevel.level);
    }

    public void UpdateTier(EnemyTier tier)
    {
        _MyTier = tier;
        UpdateStats(GetComponent<Level>().level);
    }

    //Get your initial stats from your species and your level
    public void UpdateStats(int level)
    {
        Dictionary<BaseStatSource, float[]> compoundStats = _MySpecimen.GetStatsPerCategory();

        for (int statIndex = 0; statIndex < (int)PrimaryStat.END; statIndex++)
        {
            float tierModifier = 1f;

            switch (statIndex)
            {
                case (int)PrimaryStat.HP:
                    tierModifier = EnemyTierValues.defensiveMultiplier[(int)_MyTier];
                    break;
                case (int)PrimaryStat.STR:
                    tierModifier = EnemyTierValues.offensiveMultiplier[(int)_MyTier];
                    break;
                case (int)PrimaryStat.INT:
                    tierModifier = EnemyTierValues.offensiveMultiplier[(int)_MyTier];
                    break;
                case (int)PrimaryStat.AGI:
                    tierModifier = EnemyTierValues.speedMultiplier[(int)_MyTier];
                    break;
                case (int)PrimaryStat.MOVE_SPEED:
                    tierModifier = EnemyTierValues.speedMultiplier[(int)_MyTier];
                    break;
                case (int)PrimaryStat.ANIMATION_SPEED:
                    tierModifier = EnemyTierValues.speedMultiplier[(int)_MyTier];
                    break;
            }

            float specimenStatBonus = 0;
            if (statIndex < (int)PrimaryStat.SLASH_RES)
                specimenStatBonus += compoundStats[BaseStatSource.GENOTYPE][statIndex] +
                                    compoundStats[BaseStatSource.PHENOTYPE][statIndex];

            baseStats[statIndex] = (compoundStats[BaseStatSource.SPECIES][statIndex] +
                                    specimenStatBonus) * tierModifier;

            baseStats[statIndex] += compoundStats[BaseStatSource.ITEMS][statIndex];

            if (_StatEvents[statIndex] == null)
                _StatEvents[statIndex] = new StatEvent();

            _StatEvents[statIndex].Dispatch(baseStats[statIndex]);
        }
    }

#region UPDATES
    void Update ()
    {
        UpdateModifiers();
        UpdateStatusEffects();
    }

    private void UpdateStatusEffects()
    {
        for (int i = 0; i < _StatusEffects.Length; i++)
        {
            if (_StatusEffects[i].active)
            {
                _StatusEffects[i].timeRemaining -= Time.deltaTime;
                if (_StatusEffects[i].timeRemaining < 0)
                {
                    _StatusEffects[i].timeRemaining = 0;
                    _StatusEffects[i].active = false;

                    GetComponent<CreatureFXandAnimation>().SetEffect((StatusEffectType)i, false);
                }
            }
        }
    }

    private void UpdateModifiers()
    {
        for (int i = 0; i < _Modifiers.Count; i++)
        {
            if (_Modifiers[i].timed)
            {
                if (_Modifiers[i].timeRemaining > 0)
                    _Modifiers[i].timeRemaining -= Time.deltaTime;
                else
                    RemoveBuff(_Modifiers[i]);
            }
        }
    }
#endregion

        public float GetStat(PrimaryStat stat)
    {
        return ( baseStats[(int)stat] + GetAdditiveModifierForGivenStat((int)stat) ) * GetMultiplicativeModifierForGivenStat((int)stat);
    }

    public float GetSecondaryStat( BuffType stat )
    {
        float statValue = 0;
        switch ( stat )
        {
            case BuffType.CRIT_DAMAGE:
                statValue = ((GetStat(PrimaryStat.DEX) / 500f) + GetAdditiveModifierForGivenStat((int)stat) ) * GetMultiplicativeModifierForGivenStat((int)stat);
                break;
            case BuffType.COOLDOWN:
                statValue = ( GetStat(PrimaryStat.AGI) + GetAdditiveModifierForGivenStat((int)stat)) * GetMultiplicativeModifierForGivenStat((int)stat);
                break;
            case BuffType.ATTACK_DURATION:
                statValue = (GetStat(PrimaryStat.AGI) + GetAdditiveModifierForGivenStat((int)stat)) * GetMultiplicativeModifierForGivenStat((int)stat) * (_StatusEffects[(int)StatusEffectType.FROZEN].active ? 0.7f : 1f);
                break;
            case BuffType.CRIT_BONUS:
                statValue = (GetStat(PrimaryStat.DEX) + GetAdditiveModifierForGivenStat((int)stat)) * GetMultiplicativeModifierForGivenStat((int)stat);
                break;
            case BuffType.CRIT_DEF:
                statValue = (GetStat(PrimaryStat.AGI) + GetAdditiveModifierForGivenStat((int)stat)) * GetMultiplicativeModifierForGivenStat((int)stat) * (_StatusEffects[(int)StatusEffectType.FROZEN].active ? 0.7f : 1f);
                break;
        }

        return statValue;
    }

    float GetMultiplicativeModifierForGivenStat( int stat )
    {
        float returnValue = 1;

        foreach (StatModifier buff in _Modifiers)
        {
            if ((int)buff.stat == stat && buff.operation == StatModifier.ModifierOperation.MULTIPLICATIVE)
                returnValue *= (1 + buff.value);
        }

        // STATUS EFFECTS ON STATS
        if ( ((BuffType)stat == BuffType.STR || (BuffType)stat == BuffType.INT ) && _StatusEffects[(int)StatusEffectType.BURNT].active )
            returnValue *= 0.7f;
        else if (((BuffType)stat == BuffType.WILL || (BuffType)stat == BuffType.DEF) && _StatusEffects[(int)StatusEffectType.ELECTRIFIED].active)
            returnValue *= 0.7f;

        return returnValue;
    }

    float GetAdditiveModifierForGivenStat(int stat)
    {
        float returnValue = 0;

        foreach (StatModifier buff in _Modifiers)
        {
            if ((int)buff.stat == stat && buff.operation == StatModifier.ModifierOperation.ADDITIVE)
                returnValue += buff.value;
        }

        return returnValue;
    }

    public List<StatModifier> GetStatModifiers()
    {
        return _Modifiers;
    }

    // RECEIVE HIT
    public void Hit(SkillHit hit)
    {
        if ( hit.statusEffects.Count > 0 )
        {
            foreach (StatusEffect statusEffect in hit.statusEffects)
            {
                //Update time remaining only when the effect is inactive or already active but for less time
                if (_StatusEffects[(int)statusEffect.status].active == false || _StatusEffects[(int)statusEffect.status].timeRemaining < statusEffect.duration)
                {
                    _StatusEffects[(int)statusEffect.status].active = true;
                    _StatusEffects[(int)statusEffect.status].timeRemaining = statusEffect.duration;

                    GetComponent<CreatureFXandAnimation>().SetEffect(statusEffect.status);
                }
            }
        }

        if ( hit.statModifiers.Count > 0 )
        {
            foreach ( StatModifier statModifier in hit.statModifiers )
                AddBuff(statModifier);
        }
    }

    #region Operations_over_the_buffs
    public StatModifier FindBuff(StatModifier buff)
    {
        foreach (StatModifier b in _Modifiers)
        {
            if (buff.skillRefference == b.skillRefference)
                return b;
        }
        return null;
    }

    public void AddBuff(StatModifier statModifier)
    {
        StatModifier newStatModifier = FindBuff(statModifier);
        if (newStatModifier != null)
            UpdateBuff(newStatModifier, statModifier.duration);
        else
            AddNewBuff(statModifier);

        if (statModifier.skillRefference != null && statModifier.skillRefference.targetVFX != null)
            Instantiate(statModifier.skillRefference.targetVFX, transform);
    }

    private void UpdateBuff(StatModifier newStatModifier, float duration)
    {
        if (newStatModifier.stacks < newStatModifier.maxStacks)
            newStatModifier.stacks++;
        newStatModifier.timeRemaining = duration;
        AddedBuffEvent?.Invoke(newStatModifier);
    }

    private void AddNewBuff( StatModifier statModifier)
    {
        StatModifier newStatModifier = new StatModifier(statModifier);
        newStatModifier.timeRemaining = statModifier.duration;
        newStatModifier.stacks = 1;

        _Modifiers.Add(newStatModifier);
        AddedBuffEvent?.Invoke(newStatModifier);
    }

    public void RemoveBuff (StatModifier deletedBuff)
    {
        StatModifier deletedBuffInstance = FindBuff(deletedBuff);

        if (deletedBuffInstance != null)
        {
            RemovedBuffEvent?.Invoke(FindBuff(deletedBuff));
            _Modifiers.Remove(FindBuff(deletedBuff));
        }
    }
    #endregion
}
