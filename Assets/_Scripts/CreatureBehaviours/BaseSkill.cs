using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTag
{
    NONE,
    // Skill type
    BASIC, COOLDOWN, PASSIVE,
    // Defense type
    BODY, MIND,
    // Damage type
    PIERCE, SLASH, BLUNT, FIRE, ICE, LIGHTING,
    // Body part
    CLAW, HORN, BEAK, TAIL, FEATHER,
    // Delivery
    CONTACT, PROJECTILE, AREA_OF_EFFECT,
    // Effect
    DURATION, LIFE_STEAL, MANA_STEAL,
    // Misc
    SOUND
}

public abstract class BaseSkill : ScriptableObject
{
    public static int[] MAX_SKILLS = { 1, 2, 1, 3 };
    public string description;

    public Sprite icon;

    [Header("Visual")]
    public GameObject projectile;
    public GameObject useVFX;
    public GameObject targetVFX;

    [Header("Targeting")]
    public TargetType targetType;
    public float useRange;
    public float effectRange;

    [Header("Damage")]
    public float damage;
    [Range(0, 1)]
    public float criticalChance;
    public float criticalDamage;
    public DamageType damageType;
    public DamageClass damageClass;
    public float lifeSteal;

    [Header("Timings")]
    public float cooldown;

    [Header("Extra effects")]
    // Applied to the target
    public List<StatModifier> StatModifiersForTarget;
    public List<StatusEffect> StatusEffects;
    public List<DamageOverTime> DamageOverTimeEffects;
    // Applied to the skill hit depending on target condition
    public List<ModifierOnCondition> ModifiersOnCondition;

    [Header("Sound")]
    public AudioClip impactSound;

    public List<SkillTag> tags;
}
