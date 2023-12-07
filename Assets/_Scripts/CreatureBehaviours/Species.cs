using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SkillLearn
{
    public BaseSkill skill;
    public int level;
}

[CreateAssetMenuAttribute][System.Serializable]
public class Species : ScriptableObject
{
    [CreatureStats]
    public float[] baseStats = new float[(int)PrimaryStat.END];

    [Header("Info")]
    [TextArea]
    public string description;
    public string speciesName
    {
        get
        {
            return name.Substring(name.IndexOf('_') + 1, name.Length - name.IndexOf('_') - 1);
        }
    }

    public string speciesNumber
    {
        get
        {
            return name.Substring(0, name.IndexOf('_'));
        }
    }

    [Header("Visual")]
    /// <summary>
    /// Character model including animator to be used in the creatures gameobject and as dummy for the portraits
    /// </summary>
    public GameObject model;
    /// <summary>
    /// Fullbody image
    /// </summary>
    public Sprite miniature;
    /// <summary>
    /// Close up image
    /// </summary>
    public Sprite closeUpImage;

    [Header("Audio")]
    /// <summary>
    /// Sounds played when the animator calls the "Footstep" event
    /// </summary>
    public AudioClip[] stepSounds;

    /// <summary>
    /// List of active skills stored to be learnt when the player decides to level up
    /// </summary>
    public SkillLearn[] skillList;

    /// <summary>
    /// List of types of equipment slots of the creature
    /// </summary>
    public EquipmentType[] equipmentSlots;

    /// <summary>
    /// List of possible morphs, species this one may morph into and under which conditions
    /// </summary>
    public Morph[] possibleMorphs;


    public List<BaseSkill> GetPosibleSkills(int level, bool passive = false, SkillCategory category = SkillCategory.BASIC)
    {
        List<BaseSkill> possibleSkills = new List<BaseSkill>();

        foreach (SkillLearn s in skillList)
        {
            if (s.level <= level && s.skill != null)
            {
                if (passive)
                {
                    if (s.skill is BasePassiveSkill)
                        possibleSkills.Add(s.skill);
                }
                else if ((s.skill is BaseActiveSkill) && ((BaseActiveSkill)s.skill).category == category)
                    possibleSkills.Add(s.skill);
            }
        }

        return possibleSkills;
    }

    static public List<Species> GetOriginalSpecies(Species searchSpecies)
    {
        IList<Species> allSpecies = ScriptableReferencesHolder.GetSpeciesList();

        List<Species> baseSpecies;
        List<Species> tempSpecies = new List<Species>() { searchSpecies };
        do
        {
            baseSpecies = new List<Species>(tempSpecies);
            tempSpecies = GetPreviousSpecies(searchSpecies, allSpecies);
        }
        while (tempSpecies.Count > 0);

        return baseSpecies;
    }

    static public List<Species> GetPreviousSpecies(Species originalSpecies, IList<Species> allSpecies)
    {
        List<Species> previousSpecies = new List<Species>();

        foreach (Species species in allSpecies)
        {
            if (previousSpecies.Contains(species))
                continue;

            foreach (Morph morph in species.possibleMorphs)
            {
                if (morph.targetSpecies == originalSpecies)
                    previousSpecies.Add(species);
            }
        }

        return previousSpecies;
    }
}
