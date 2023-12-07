using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAreaDamage : PassiveSkill
{
    public CreatureStats _MyStats;

    public List<StatModifier> _StatModifiers = new List<StatModifier>();
    public List<StatusEffect> _StatusEffects = new List<StatusEffect>();
    public List<DamageOverTime> _DamageOverTimeEffects = new List<DamageOverTime>();

    public GameObject _FX;

    public float timeCounter = 0f;

    private void Awake()
    {
        if (_MyStats == null)
            _MyStats = GetComponent<CreatureStats>();
    }

    protected override void Set()
    {
        if (_FX == null && _MyBasePassive != null && _MyBasePassive.useVFX != null)
            _FX = Instantiate(_MyBasePassive.useVFX, transform);

        OnEnable();
    }

    private void OnEnable()
    {
        if (_FX != null)
            _FX.SetActive(true);
    }

    private void OnDisable()
    {
        if (_FX != null)
            _FX.SetActive(false);
    }

    private void Update()
    {
            if (timeCounter <= 0)
            {
                timeCounter = _MyBasePassive.cooldown;
                Execute();
            }
            else
                timeCounter -= Time.deltaTime;
    }

    protected SkillHit GetSkillHit()
    {
        SkillHit hit = new SkillHit();

        hit.damageInstances = new DamageInstance[1];
        hit.damageInstances[0].damage = _MyBasePassive.damage;
        hit.damageInstances[0].type = _MyBasePassive.damageType;

        hit.strength = _MyStats.GetStat(PrimaryStat.STR);
        hit.intelligence = _MyStats.GetStat(PrimaryStat.INT);

        hit.baseCritChance = 0;
        hit.dextrity = _MyStats.GetStat(PrimaryStat.DEX);

        //Get all the effects of the skill
        hit.statModifiers = new List<StatModifier>(_MyBasePassive.StatModifiersForTarget);
        hit.statusEffects = new List<StatusEffect>(_MyBasePassive.StatusEffects);
        hit.damageOverTimeEffects = new List<DamageOverTime>(_MyBasePassive.DamageOverTimeEffects);
        hit.targetBasedModifiers = new List<ModifierOnCondition>(_MyBasePassive.ModifiersOnCondition);

        hit.attacker = this.gameObject;

        return hit;
    }

    private void Execute()
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _MyBasePassive.effectRange);

        SkillHit hit = GetSkillHit();
        foreach (Collider c in posibleTargets)
        {
            if ((gameObject.tag == "Enemy" && c.tag == "Player") ||
                 (gameObject.tag == "Player" && c.tag == "Enemy"))
            {
                c.SendMessage("Hit", hit);
            }
        }
    }
}
