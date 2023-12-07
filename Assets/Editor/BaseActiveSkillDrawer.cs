using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseActiveSkill))]
[CanEditMultipleObjects]
public class CustomClassView : Editor
{
    SerializedProperty skillImplementation;
    SerializedProperty category;

    SerializedProperty icon;
    SerializedProperty description;

    [Header("Visual")]
    SerializedProperty projectile;
    SerializedProperty useVisualEffect;
    SerializedProperty useVFX;
    SerializedProperty targetVFX;
    SerializedProperty animationTrigger;

    [Header("Targeting")]
    SerializedProperty targetType;
    SerializedProperty useRange;
    SerializedProperty effectRange;

    [Header("Damage")]
    SerializedProperty damage;
    SerializedProperty criticalChance;
    SerializedProperty criticalDamage;
    SerializedProperty damageType;
    SerializedProperty damageClass;
    SerializedProperty lifeSteal;

    SerializedProperty manaCost;

    [Header("Timings")]
    SerializedProperty cooldown;
    SerializedProperty executionDuration;
    SerializedProperty hitInstant;
    SerializedProperty creationsDuration;

    [Header("Extra effects")]
    SerializedProperty StatusEffects;
    SerializedProperty DamageOverTimeEffects;
    SerializedProperty StatModifiersForTarget;
    SerializedProperty ModifiersOnCondition;

    [Header("Sound")]
    SerializedProperty impactSound;
    SerializedProperty castingSound;

    SerializedProperty tags;

    void OnEnable()
    {
        skillImplementation = serializedObject.FindProperty("skillImplementation");
        category = serializedObject.FindProperty("category");

        icon = serializedObject.FindProperty("icon");
        description = serializedObject.FindProperty("description");

        projectile = serializedObject.FindProperty("projectile");
        useVFX = serializedObject.FindProperty("useVFX");
        targetVFX = serializedObject.FindProperty("targetVFX");
        animationTrigger = serializedObject.FindProperty("animationTrigger");

        targetType = serializedObject.FindProperty("targetType");
        useRange = serializedObject.FindProperty("useRange");
        effectRange = serializedObject.FindProperty("effectRange");

        damage = serializedObject.FindProperty("damage");
        criticalChance = serializedObject.FindProperty("criticalChance");
        criticalDamage = serializedObject.FindProperty("criticalDamage");
        damageType = serializedObject.FindProperty("damageType");
        damageClass = serializedObject.FindProperty("damageClass");
        lifeSteal = serializedObject.FindProperty("lifeSteal");

        manaCost = serializedObject.FindProperty("manaCost");

        cooldown = serializedObject.FindProperty("cooldown");
        executionDuration = serializedObject.FindProperty("executionDuration");
        hitInstant = serializedObject.FindProperty("hitInstant");
        creationsDuration = serializedObject.FindProperty("creationsDuration");

        StatusEffects = serializedObject.FindProperty("StatusEffects");
        DamageOverTimeEffects = serializedObject.FindProperty("DamageOverTimeEffects");
        StatModifiersForTarget = serializedObject.FindProperty("StatModifiersForTarget");
        ModifiersOnCondition = serializedObject.FindProperty("ModifiersOnCondition");

        impactSound = serializedObject.FindProperty("impactSound");
        castingSound = serializedObject.FindProperty("castingSound");

        tags = serializedObject.FindProperty("tags");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(skillImplementation);
        EditorGUILayout.PropertyField(category);

        EditorGUILayout.PropertyField(icon);
        EditorGUILayout.PropertyField(description);

        EditorGUILayout.PropertyField(projectile);
        EditorGUILayout.PropertyField(useVFX);
        EditorGUILayout.PropertyField(targetVFX);
        EditorGUILayout.PropertyField(animationTrigger);

        EditorGUILayout.PropertyField(targetType);
        EditorGUILayout.PropertyField(useRange);
        EditorGUILayout.PropertyField(effectRange);

        EditorGUILayout.PropertyField(damage);
        EditorGUILayout.PropertyField(criticalChance);
        EditorGUILayout.PropertyField(criticalDamage);
        EditorGUILayout.PropertyField(damageType);
        EditorGUILayout.PropertyField(damageClass);
        EditorGUILayout.PropertyField(lifeSteal);

        EditorGUILayout.PropertyField(manaCost);

        EditorGUILayout.PropertyField(cooldown);
        EditorGUILayout.PropertyField(executionDuration);
        EditorGUILayout.PropertyField(hitInstant);
        EditorGUILayout.PropertyField(creationsDuration);

        EditorGUILayout.PropertyField(StatusEffects, new GUIContent("Status effects list"), true);
        EditorGUILayout.PropertyField(DamageOverTimeEffects, new GUIContent("Damage over time list"), true);
        EditorGUILayout.PropertyField(StatModifiersForTarget, new GUIContent("Stat modifiers for target list"), true);
        EditorGUILayout.PropertyField(ModifiersOnCondition, new GUIContent("Modifiers on condition list"), true);

        EditorGUILayout.PropertyField(impactSound);
        EditorGUILayout.PropertyField(castingSound);

        EditorGUILayout.PropertyField(tags, new GUIContent("Tag list"), true);

        serializedObject.ApplyModifiedProperties();
    }

}
