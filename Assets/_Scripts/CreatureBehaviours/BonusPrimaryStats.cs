using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BonusPrimaryStats
{
    public const float MAX_GENOTYPE_PER_STAT = 15f;
    public const float MAX_COMBINED_PHENOTYPE = 50f;
    public const float MAX_PHENOTYPE_PER_STAT = 25f;

    [SerializeField] private float[] stats = new float[(int)PrimaryStat.SLASH_RES];

    public delegate void StatsChanged(float[] newStats);
    public event StatsChanged StatsChangedEvent;

    public float this[int i]
    {
        get { return stats[i]; }
        private set { stats[i] = value; }
    }

    public float this[PrimaryStat stat]
    {
        get { return this[(int)stat]; }
        private set { this[(int)stat] = value; }
    }

    #region CONSTRUCTORS
    public BonusPrimaryStats(float[] setStats)
    {
        SetStats(setStats);
    }

    public BonusPrimaryStats(BonusPrimaryStats setStats)
    {
        SetStats(setStats);
    }

    public BonusPrimaryStats()
    { }
    #endregion

    #region GETTERS AND SETTERS
    public void SetStats(float[] setStats)
    {
        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
            stats[i] = setStats[i];

        StatsChangedEvent?.Invoke(GetStats());
    }

    public void SetStats(BonusPrimaryStats setStats)
    {
        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
            stats[i] = setStats[i];

        StatsChangedEvent?.Invoke(GetStats());
    }

    public float[] GetStats()
    {
        float[] returnFloat = new float[stats.Length];
        stats.CopyTo(returnFloat, 0);
        return returnFloat;
    } 
    #endregion

    public void RollStats(Dictionary<PrimaryStat, float> statIncreases)
    {
        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
        {
            float increase = 0;
            if (statIncreases.ContainsKey((PrimaryStat)i))
                increase = statIncreases[(PrimaryStat)i];

            stats[i] = Random.Range( increase, MAX_GENOTYPE_PER_STAT);
        }

        StatsChangedEvent?.Invoke(GetStats());
    }

    public void RollStats()
    {
        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
            stats[i] = Random.value * MAX_GENOTYPE_PER_STAT;

        StatsChangedEvent?.Invoke(GetStats());
    }

    public void SetStatsFractionOfMax(float fraction)
    {
        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
            stats[i] = fraction * MAX_GENOTYPE_PER_STAT;

        StatsChangedEvent?.Invoke(GetStats());
    }

    public void GainStat(PrimaryStat increasedStat, float addedValue)
    {
        float remainingCombinedPhenotype = MAX_COMBINED_PHENOTYPE;

        for (int i = 0; i < (int)PrimaryStat.SLASH_RES; i++)
            remainingCombinedPhenotype -= stats[i];

        float remainingPhenotypeForStat = MAX_PHENOTYPE_PER_STAT - stats[(int)increasedStat];

        float possibleIncrease = Mathf.Min(remainingCombinedPhenotype, remainingPhenotypeForStat, addedValue);

        if (possibleIncrease > 0)
            stats[(int)increasedStat] += possibleIncrease;

        StatsChangedEvent?.Invoke(GetStats());
    }

    public void LoseStat(PrimaryStat increasedStat, float lostValue)
    {
        if (lostValue < 0)
            Debug.LogError("Attempting to substract a negative value from bonus stats");

        if (stats[(int)increasedStat] > lostValue)
            stats[(int)increasedStat] -= lostValue;
        else
            stats[(int)increasedStat] = 0;

        StatsChangedEvent?.Invoke(GetStats());
    }

}
