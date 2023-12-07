using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTier
{
    FAINT,
    SWARM,
    STANDARD,
    STERN,
    RAGING,
    VIGOROUS,
    BETA,
    ALPHA,
    END
}

public static class EnemyTierValues
{
    //                                              faint   swarm   standrd stern   raging  vigoros beta    alpha
    public static float[] sizeMultiplier      = {   1f,     0.7f,   1f,     1.3f,   1.1f,   1.2f,   1.2f,   1.4f    };
    public static float[] offensiveMultiplier = {   0.7f,   0.7f,   1f,     1f   ,  2f,   3f  ,   2f ,    2f      };
    public static float[] defensiveMultiplier = {   0.8f,   0.5f,   1f,     3f   ,  1f,     3f  ,   5f  ,   6f     };
    public static float[] speedMultiplier     = {   0.8f,   2.5f,    1f,     0.8f ,  2f,   1.3f,   1.3f ,  1.5f      };
    public static int[]   spawnNumber         = {   2,      5,      2,     1    ,  3,      3   ,   3  ,    1       };
    public static float[] experienceMultiplier ={   0.7f,   0.6f,   1f,    3f   ,  2f,   5f  ,   7f ,    25      };
    public static float[] spawnChance         = {   1,      1,      1,      0,      0,      0f,     0f,     1f      };
    //                                              faint   swarm   standrd stern   raging  vigoros beta    alpha
    public static Color[] color = { Color.black, Color.black, Color.black, Color.green, Color.red, Color.blue, Color.yellow, Color.yellow };
}

public class EnemySpawner : MonoBehaviour
{
    private enum EnemyPreset
    {
        SternAndRaging,
        AlphaAndBetas,
        Vigorous,
        Other
    }
    private float[] EnemyPresetChance = { 0.25f, 0.05f, 0.1f, 0.6f };

    public float detectionRadius = 20f;

    public GameObject baseEnemy;


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.GetComponent<CreatureStats>() != null)
        {
            Spawn();
            gameObject.SetActive(false);
        }
    }

    private void Spawn()
    {
        EnemyTier[] tiers = GetPreset();

        Species enemySpecies = GetRandomWeigthedSpecies( DataManager._CurrentArea.species );

        float playerCreatureAmountModifier = ((float)GameManager.playerCreatures.Count) * 0.25f;
        int quantity = EnemyTierValues.spawnNumber[(int)tiers[0]] + EnemyTierValues.spawnNumber[(int)tiers[1]];

        int areaLevel = DataManager._CurrentArea.areaLevel;
        List<BaseSkill> possibleBasicSkills = enemySpecies.GetPosibleSkills(areaLevel, false, SkillCategory.BASIC);
        List<BaseSkill> possibleCooldownSkills = enemySpecies.GetPosibleSkills(areaLevel, false, SkillCategory.COOLDOWN);
        List<BaseSkill> possiblePassiveSkills = enemySpecies.GetPosibleSkills(areaLevel, true);

        for (int i = 0; i < quantity; i++)
        {
            Specimen newSpecimen = GetSpecimen(enemySpecies, possibleBasicSkills, possibleCooldownSkills, possiblePassiveSkills);

            EnemyTier tier = (i < EnemyTierValues.spawnNumber[(int)tiers[0]]) ? tiers[0] : tiers[1];
            GameObject newEnemy = CreatureFactory.CreateCreature(newSpecimen, baseEnemy, transform.position, i, tier);
        }
    }

    private EnemyTier[] GetPreset()
    {
        float chance = UnityEngine.Random.value;
        EnemyPreset preset = EnemyPreset.Other;

        for (int i = 0; i < Enum.GetValues(typeof(EnemyPreset)).Length; i++)
        {
            if (chance < EnemyPresetChance[i])
            {
                preset = (EnemyPreset)i;
                break;
            }
            chance -= EnemyPresetChance[i];
        }

        EnemyTier[] tier = new EnemyTier[2];
        switch (preset)
        {
            case EnemyPreset.AlphaAndBetas:
                tier[0] = EnemyTier.ALPHA;
                tier[1] = EnemyTier.BETA;
                break;
            case EnemyPreset.SternAndRaging:
                tier[0] = EnemyTier.RAGING;
                tier[1] = EnemyTier.STERN;
                break;
            case EnemyPreset.Vigorous:
                tier[0] = EnemyTier.VIGOROUS;
                tier[1] = EnemyTier.VIGOROUS;
                break;
            default:
                tier[0] = GetTier();
                tier[1] = GetTier();
                break;
        };

        return tier;
    }

    private Specimen GetSpecimen(Species enemySpecies, List<BaseSkill> possibleBasicSkills, List<BaseSkill> possibleCooldownSkills, List<BaseSkill> possiblePassiveSkills)
    {
        Specimen newSpecimen = new Specimen
        {
            exp = Level.ExperienceFunction(DataManager._CurrentArea.areaLevel, true),
            species = enemySpecies,
            secondaryColor = GetSecondaryColor(),
            name = enemySpecies.speciesName,
            genotype = new BonusPrimaryStats(),
            phenotype = new BonusPrimaryStats(),
        };
        newSpecimen.genotype.RollStats();

        List<BaseSkill> localPossibleBasicSkills = new List<BaseSkill>(possibleBasicSkills);
        List<BaseSkill> localPossibleCooldownSkills = new List<BaseSkill>(possibleCooldownSkills);
        List<BaseSkill> localPossiblePassiveSkills = new List<BaseSkill>(possiblePassiveSkills);

        //Adding Skills to the specimen
        if (localPossibleBasicSkills != null && localPossibleBasicSkills.Count > 0)
        {
            newSpecimen.basicAttack = (BaseActiveSkill)localPossibleBasicSkills[Mathf.FloorToInt(UnityEngine.Random.value * localPossibleBasicSkills.Count)];
        }
        if (localPossibleCooldownSkills != null && localPossibleCooldownSkills.Count > 0)
        {
            newSpecimen.cooldownAttack_1 = (BaseActiveSkill)localPossibleCooldownSkills[Mathf.FloorToInt(UnityEngine.Random.value * localPossibleCooldownSkills.Count)];
            localPossibleCooldownSkills.Remove(newSpecimen.cooldownAttack_1);
            if (localPossibleCooldownSkills.Count > 0)
                newSpecimen.cooldownAttack_2 = (BaseActiveSkill)localPossibleCooldownSkills[Mathf.FloorToInt(UnityEngine.Random.value * localPossibleCooldownSkills.Count)];
        }
        if (localPossiblePassiveSkills != null && localPossiblePassiveSkills.Count > 0)
        {
            newSpecimen.passive_1 = (BasePassiveSkill)possiblePassiveSkills[Mathf.FloorToInt(UnityEngine.Random.value * localPossiblePassiveSkills.Count)];
            localPossiblePassiveSkills.Remove(newSpecimen.passive_1);
            if (localPossiblePassiveSkills != null && localPossiblePassiveSkills.Count > 0)
            {
                newSpecimen.passive_2 = (BasePassiveSkill)localPossiblePassiveSkills[Mathf.FloorToInt(UnityEngine.Random.value * localPossiblePassiveSkills.Count)];
                localPossiblePassiveSkills.Remove(newSpecimen.passive_2);

                if (localPossiblePassiveSkills != null && localPossiblePassiveSkills.Count > 0)
                    newSpecimen.passive_3 = (BasePassiveSkill)localPossiblePassiveSkills[Mathf.FloorToInt(UnityEngine.Random.value * localPossiblePassiveSkills.Count)];
            }
        }

        return newSpecimen;
    }

    private EnemyTier GetTier()
    {
        EnemyTier tier = EnemyTier.STANDARD;

        float random = UnityEngine.Random.value;
        float[] chance = EnemyTierValues.spawnChance;

        float total = 0;
        for (int i = 0; i < chance.Length; i++)
            total += chance[i];

        float accumulatedChance = chance[0];

        for (int i = 0; i < (int)EnemyTier.END; i++ )
        {
            if (random < (accumulatedChance / total))
                return (EnemyTier)i;
            else
                accumulatedChance += chance[i];
        }

        return tier;
    }

    private Species GetRandomWeigthedSpecies( List<SpeciesSpawnRate> spawnRates )
    {
        Species species = null;

        float random = UnityEngine.Random.value;

        float total = 0;
        for (int i = 0; i < spawnRates.Count; i++)
            total += spawnRates[i].spawnRate;

        float accumulatedChance = spawnRates[0].spawnRate;

        for (int i = 0; i < spawnRates.Count; i++)
        {
            if (random < (accumulatedChance / total))
                return spawnRates[i].species;
            else
                accumulatedChance += spawnRates[i].spawnRate;
        }

        return species;
    }

    private Color GetSecondaryColor()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value );
    }
}
