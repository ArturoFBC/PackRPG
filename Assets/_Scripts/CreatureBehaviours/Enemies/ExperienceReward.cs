using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatureStats))]
[RequireComponent(typeof(CreatureHitPoints))]
public class ExperienceReward : MonoBehaviour
{
    EnemyTier _MyTier;

    public void SetTier( EnemyTier tier )
    {
        _MyTier = tier;
    }

    void Start()
    {
        //Subscribe to death
        GetComponent<CreatureHitPoints>().KnockOutEvent -= OnDeath;
        GetComponent<CreatureHitPoints>().KnockOutEvent += OnDeath;
    }

    void OnDeath()
    {
        float exp = 0;

        CreatureStats stats = GetComponent<CreatureStats>();

        for ( int i = 0; i < (int)PrimaryStat.SLASH_RES; i++ )
        {
            exp += stats.baseStats[i];
        }

        exp /= 40f;

        exp /= GameManager.playerCreatures.Count;

        //Apply the enemy tier experience bonus
        if ( (int)_MyTier < EnemyTierValues.experienceMultiplier.Length)
            exp *= EnemyTierValues.experienceMultiplier[(int)_MyTier];

        foreach ( GameObject go in GameManager.playerCreatures )
        {
            go.GetComponent<Level>().AddExp(exp);
        }
    }
	

}
