using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialCreatureChooseMenu : MonoBehaviour
{
    [SerializeField] List<Species> initialSpecies;
    [SerializeField] List<Specimen> initialCreatures;

    [SerializeField] int initialCreatureLevel = 5;

    [SerializeField] Transform[] initialLocations;
    [SerializeField] UIElementFollow[] elementFollows;
    [SerializeField] SpecimenTooltipDisplayer[] creatureTooltipDisplayers;
    [SerializeField] Animator[] creatureAnimators;

    [SerializeField] GameObject tooltipBoxPrefab;

    private void Awake()
    {
        CreateSpecimens();

        DisplaySpecimens();
    }

    private void CreateSpecimens()
    {
        initialCreatures = new List<Specimen>();
        for (int i = 0; i < initialSpecies.Count; i++)
        {
            Specimen newSpecimen = new Specimen();

            newSpecimen.exp = Level.ExperienceFunction(initialCreatureLevel, true);
            newSpecimen.name = initialSpecies[i].speciesName;
            newSpecimen.species = initialSpecies[i];
            newSpecimen.SetGenotype(2f / 3f);

            foreach (EquipmentType equipmentType in initialSpecies[i].equipmentSlots)
                newSpecimen.equipmentSlots.Add(new EquipmentSlot() { type = equipmentType } );

            SetSkills(initialSpecies[i], newSpecimen);

            initialCreatures.Add(newSpecimen);
        }
    }

    private void SetSkills(Species species, Specimen newSpecimen)
    {
        //Set skills
        List<BaseSkill> possibleBasicSkills = species.GetPosibleSkills(initialCreatureLevel, false, SkillCategory.BASIC);
        List<BaseSkill> possibleCooldownSkills = species.GetPosibleSkills(initialCreatureLevel, false, SkillCategory.COOLDOWN);
        List<BaseSkill> possiblePassiveSkills = species.GetPosibleSkills(initialCreatureLevel, true);

        if (possibleBasicSkills != null && possibleBasicSkills.Count > 0)
            newSpecimen.basicAttack = (BaseActiveSkill)possibleBasicSkills[0];

        if (possibleCooldownSkills != null && possibleCooldownSkills.Count > 0)
        {
            newSpecimen.cooldownAttack_1 = (BaseActiveSkill)possibleCooldownSkills[0];
            if (possibleCooldownSkills.Count > 1)
                newSpecimen.cooldownAttack_2 = (BaseActiveSkill)possibleCooldownSkills[1];
        }

        if (possiblePassiveSkills != null && possiblePassiveSkills.Count > 0)
        {
            newSpecimen.passive_1 = (BasePassiveSkill)possiblePassiveSkills[0];
            if (possiblePassiveSkills.Count > 1)
            {
                newSpecimen.passive_2 = (BasePassiveSkill)possiblePassiveSkills[1];
                if (possiblePassiveSkills.Count > 2)
                    newSpecimen.passive_3 = (BasePassiveSkill)possibleCooldownSkills[2];
            }
        }
    }

    private void DisplaySpecimens()
    {
        for (int i = 0; i < initialCreatures.Count; i++)
        {
            GameObject newInitialCreatureDummy;
            if (initialCreatures[i].species.model != null)
                newInitialCreatureDummy = Instantiate(initialCreatures[i].species.model, initialLocations[i].position, initialLocations[i].rotation);
            else
                newInitialCreatureDummy = Instantiate(new GameObject(), initialLocations[i].position, initialLocations[i].rotation);

            elementFollows[i].SetFollowing(newInitialCreatureDummy.transform);
            creatureTooltipDisplayers[i].SetObjectToDisplay(initialCreatures[i]);
            creatureAnimators[i] = newInitialCreatureDummy.GetComponentInChildren<Animator>();
        }
    }

    public void SelectInitial(int index)
    {
        CreatureStorage.activePack.Add( initialCreatures[index] );

        creatureAnimators[index]?.Play("ClawAttack");
        //DataManager.SaveData();
        SceneLoader._Reference.GoToLair();
    }
}
