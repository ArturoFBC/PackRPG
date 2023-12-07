using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudCreature : MonoBehaviour
{
    private CreatureSkillManager _MyCreatureSkillManager;
    private List<SkillIcon> _SkillIcons;

    private void Awake()
    {
        if ( _SkillIcons == null )
            GetSkillIcons();
    }

    public void Set( GameObject creature )
    {
        if (_SkillIcons == null)
            GetSkillIcons();

        _MyCreatureSkillManager = creature.GetComponent<CreatureSkillManager>();
        _MyCreatureSkillManager.SkillsChangedEvent -= AssignSkills;
        _MyCreatureSkillManager.SkillsChangedEvent += AssignSkills;
        AssignSkills();

        CreatureAvatar avatar = GetComponentInChildren<CreatureAvatar>();
        if (avatar != null)
            avatar.Set(creature);
    }

    private void GetSkillIcons()
    {
        _SkillIcons = new List<SkillIcon>();
        for (int j = 0; j < BaseSkill.MAX_SKILLS[ (int)SkillCategory.COOLDOWN ]; j++)
        {
            string s = "SkillIcon" + (j + 1).ToString();
            Transform skillIconTransform = transform.Find(s);
            if (skillIconTransform != null)
            {
                SkillIcon skillIcon = skillIconTransform.GetComponent<SkillIcon>();
                if (skillIcon != null)
                {
                    _SkillIcons.Add(skillIcon);
                }
            }
        }
    }

    private void AssignSkills()
    {
        List<Skill> cooldownSkills = _MyCreatureSkillManager.GetCooldownSkills();

        for (int skillIndex = 0; skillIndex < _SkillIcons.Count; skillIndex++)
        {
            if (skillIndex < cooldownSkills.Count)
            { 
                _SkillIcons[skillIndex].gameObject.SetActive(true);
                _SkillIcons[skillIndex].Set(cooldownSkills[skillIndex]);
            }
            else
                _SkillIcons[skillIndex].gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if ( _MyCreatureSkillManager != null )
            _MyCreatureSkillManager.SkillsChangedEvent -= AssignSkills;
    }


}
