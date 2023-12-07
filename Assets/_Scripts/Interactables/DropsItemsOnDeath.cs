using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CreatureHitPoints))]
public class DropsItemsOnDeath : DropsItems
{
    EnemyTier _MyTier;

    public void SetTier(EnemyTier tier)
    {
        _MyTier = tier;
    }

    void Start()
    {
        //Subscribe to death
        GetComponent<CreatureHitPoints>().KnockOutEvent -= OnDeath;
        GetComponent<CreatureHitPoints>().KnockOutEvent += OnDeath;
    }

    private void OnDestroy()
    {
        if (GetComponent<CreatureHitPoints>() != null)
            GetComponent<CreatureHitPoints>().KnockOutEvent -= OnDeath;
    }

    public void OnDeath()
    {
        DropItems();
    }

    protected override List<Drop> GetDrops()
    {
        List<Drop> drops = GetEssenceDrops();

        return drops;
    }

    private List<Drop> GetEssenceDrops()
    {
        int dropAmount = 3;

        //Apply the enemy tier experience bonus
        if ((int)_MyTier < EnemyTierValues.experienceMultiplier.Length)
            dropAmount = Mathf.RoundToInt((float)dropAmount * EnemyTierValues.experienceMultiplier[(int)_MyTier]);

        Species species = GetComponent<CreatureStats>().GetSpecimen().species;
        List<Essence> essences = Essence.GetEssences(species);

        List<Drop> drops = new List<Drop>();
        foreach (Essence essence in essences)
        {
            drops.Add(new Drop()
            {
                dropable = essence,
                amount = dropAmount / essences.Count
            });
        }

        return drops;
    }
}
