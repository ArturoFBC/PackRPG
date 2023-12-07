using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RolePlayingFormulas
{
    //This provides scaling with level plus a base wich is dependent on base stats from level one
    public static float HPfromHPStat(float hpStat, int level)
    {
        return hpStat + (hpStat / level) * 20f;
    }

    public static float DamageCalculation(float rawDamage, float attack, float defense)
    {
        return rawDamage * (1 + (attack / 10)) / (1 + (defense / 10));
    }

    public static float CriticalChanceCalculation(float baseChance, float accuracy, float evasion)
    {
        return baseChance * Mathf.Pow((accuracy / evasion), 1.6f);
    }

    public static float CriticalBonusCalculation(float dextrity, float criticalDamageBonus)
    {
        return 2 + (dextrity / 1000f) + criticalDamageBonus;
    }

    public static float HealCalculation( float baseHeal, float healerBonus, float healedBonus)
    {
        return baseHeal * (1 + healerBonus) * (1 + healedBonus);
    }

    public static float TerapeucityFromWill( float will )
    {
        return will / 200;
    }

    public static float DropRateCalculation(float dropRarity, float sourceRarityBonus)
    {
        float rarityCoeficient = 1f / (1f + sourceRarityBonus);
        return Mathf.Pow(3f * dropRarity / rarityCoeficient, rarityCoeficient) / 3f;
    }

    public static float[] ApplyLevelToBaseStats( float[] stats, int level)
    {
        float[] returnStats = new float[ stats.Length ];

        // Only primary stats get multiplied by level, resistances dont
        for (int i = 0; i < stats.Length; i++)
            returnStats[i] = ( i < (int)PrimaryStat.SLASH_RES ) ? ApplyLevelToBaseStat(stats[i], level) : stats[i];

        return returnStats;
    }

    public static float ApplyLevelToBaseStat(float stat, int level)
    {
        return stat * 0.1f * level;
    }

    public static bool HealthManaGlobesDrop( List<float> fillPercentages )
    {
        float dropChance = 0;
        foreach (float fillPercent in fillPercentages)
        {
            float chance = 1 - (fillPercent * 3);
            if (chance > 0)
                dropChance += chance;
        }

        if (Random.value < dropChance)
            return true;

        return false;
    }
}
