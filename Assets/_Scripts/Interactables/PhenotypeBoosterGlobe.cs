using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhenotypeBoosterGlobe : PickUp
{
    const float phenoAmount = 10f;

    private PrimaryStat myPrimaryStat;

    private void Awake()
    {
        myPrimaryStat = (PrimaryStat)Random.Range( 0, (int)PrimaryStat.MP );
        gameObject.name = "+" + phenoAmount.ToString() + " " + myPrimaryStat.ToString() + " booster";
    }

    public override void Interact(Transform whoActivatedMe)
    {
        CreatureStats activatorStats = whoActivatedMe.GetComponent<CreatureStats>();

        if (activatorStats != null)
        {
            activatorStats.GetSpecimen().phenotype.GainStat(myPrimaryStat, phenoAmount);
        }

        DisplayVisualEffects(whoActivatedMe);
        DisplayAudioEffect();
    }
}
