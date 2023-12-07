using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnSkillButton : MonoBehaviour
{
    private CreatureStats _MyCreatureStats;

    public void Set(CreatureStats creatureStats)
    {
        _MyCreatureStats = creatureStats;
        if (_MyCreatureStats)
            CheckForSkills();
        else
            gameObject.SetActive(false);
    }

    //Show the button if there are skills to learn
    public void CheckForSkills(int i = 0)
    {
        CheckForSkills();
    }

    public void CheckForSkills()
    {
        if (_MyCreatureStats.GetSpecimen().skillsToLearn.Count > 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        // Gather data for the skill panel and open it

        Specimen creatureLearning = _MyCreatureStats.GetSpecimen();

        if (creatureLearning.skillsToLearn.Count > 0)
        {
            BaseSkill newSkill = creatureLearning.skillsToLearn[0];
            List<BaseSkill> knownSkills = new List<BaseSkill>();


            if (newSkill is BaseActiveSkill)
            {
                switch (((BaseActiveSkill)newSkill).category)
                {
                    case SkillCategory.BASIC:
                        knownSkills.Add(creatureLearning.basicAttack);
                        break;
                    case SkillCategory.COOLDOWN:
                        knownSkills.Add(creatureLearning.cooldownAttack_1);
                        knownSkills.Add(creatureLearning.cooldownAttack_2);
                        break;
                }
            }
            else if ( newSkill is BasePassiveSkill )
            {
                knownSkills.Add(creatureLearning.passive_1);
                knownSkills.Add(creatureLearning.passive_2);
                knownSkills.Add(creatureLearning.passive_3);
            }

            InGameGUIManager._Ref.OpenSkillLearnPanel( _MyCreatureStats.gameObject.GetComponent<CreatureSkillManager>(), newSkill, knownSkills );
        }
    }
}
