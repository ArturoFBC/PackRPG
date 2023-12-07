using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phenotype : BonusPrimaryStats
{
    public Phenotype(float[] setStats) : base(setStats)
    {
        SetStats(setStats);
    }


}
