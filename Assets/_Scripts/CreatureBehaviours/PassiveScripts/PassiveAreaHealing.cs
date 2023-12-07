using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAreaHealing : PassiveSkill
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

    protected HealHit GetHealInstance()
    {
        HealHit heal = new HealHit();

        heal.baseHeal = _MyBasePassive.damage;

        heal.terapeucity = RolePlayingFormulas.TerapeucityFromWill(_MyStats.GetStat(PrimaryStat.WILL));

        //Get all the effects of the skill
        heal.statModifiers = new List<StatModifier>(_MyBasePassive.StatModifiersForTarget);
        heal.statusEffects = new List<StatusEffect>(_MyBasePassive.StatusEffects);
        heal.damageOverTimeEffects = new List<DamageOverTime>(_MyBasePassive.DamageOverTimeEffects);
        heal.targetBasedModifiers = new List<ModifierOnCondition>(_MyBasePassive.ModifiersOnCondition);

        heal.healer = this.gameObject;

        return heal;
    }

    private void Execute()
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _MyBasePassive.effectRange);

        HealHit hit = GetHealInstance();
        foreach (Collider c in posibleTargets)
        {
            if (gameObject.tag == c.tag)
            {
                c.SendMessage("Heal", hit);
            }
        }
    }
}
