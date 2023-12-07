using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MorphConditionType
{
    LEVEL,
    END
}

[System.Serializable]
public struct MorphCondition
{
    public MorphConditionType type;
    public int value;
}

[System.Serializable]
public struct MorphEssenceCost
{
    public Species species;
    public int amount;
}

[System.Serializable]
public struct Morph
{
    public MorphCondition[] morphConditions;
    public MorphEssenceCost[] morphEssenceCosts;
    public Species targetSpecies;
}

public struct MorphStatus
{
    public Morph morph;
    public bool creatureReady;
    public bool costAfforadable;
}

public static class CreatureMorph
{
    static public List<MorphStatus> CheckForAvailableMorph( Specimen mySpecimen )
    {
        List<MorphStatus> morphStatus = new List<MorphStatus>();

        Species _MySpecies = mySpecimen.species;

        //Check if there are any possible morphs
        if (_MySpecies.possibleMorphs == null || _MySpecies.possibleMorphs.Length == 0)
            return morphStatus;

        foreach (Morph morph in _MySpecies.possibleMorphs)
        {
            MorphStatus status = new MorphStatus();
            status.morph = morph;
            status.creatureReady = true;
            status.costAfforadable = true;

            //Check if creature is ready
            foreach (MorphCondition condition in morph.morphConditions)
            {
                switch ( condition.type )
                {
                    case MorphConditionType.LEVEL:
                        if ( mySpecimen.exp < Level.ExpLevels[condition.value] )
                            status.creatureReady = false;
                    break;
                }
            }

            //Check if enough currency is available
            foreach (MorphEssenceCost cost in morph.morphEssenceCosts)
            {
                if ( !InventoryManager.Ref._SpecificEssences.ContainsKey( cost.species ) ||
                    cost.amount < InventoryManager.Ref._SpecificEssences[ cost.species ])
                {
                    status.costAfforadable = false;
                    break;
                }
            }

            morphStatus.Add(status);
        }

        return morphStatus;
    }
}
