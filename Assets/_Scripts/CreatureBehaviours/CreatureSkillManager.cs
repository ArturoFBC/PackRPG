using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSkillManager : MonoBehaviour
{
    public delegate void SkillsChanged();
    public event SkillsChanged SkillsChangedEvent;

    private Skill _BasicSkill;
    private List<Skill> _CooldownSkills = new List<Skill>();
    private List<PassiveSkill> _PassiveSkills = new List<PassiveSkill>();

    private void Start()
    {
        // Subscribe to levelUp event to update skills to be learnt according to level up
        Level myLevel = GetComponent<Level>();
        if (myLevel != null)
            myLevel.levelUpEvent += AddSkillsToLearn;

        // If it is a player creature, I can get the specimen and work with that
        CreatureStats myStats = GetComponent<CreatureStats>();
        if ( myStats != null )
            AssignSkills( myStats.GetSpecimen() );
    }

    private void AssignSkills( Specimen mySpecimen )
    {
        _BasicSkill = Skill.AssignToCreature(gameObject, mySpecimen.basicAttack);

        _CooldownSkills.Clear();
        if (mySpecimen.cooldownAttack_1 != null)
            _CooldownSkills.Add( Skill.AssignToCreature(gameObject, mySpecimen.cooldownAttack_1) );

        if (mySpecimen.cooldownAttack_2 != null)
            _CooldownSkills.Add( Skill.AssignToCreature(gameObject, mySpecimen.cooldownAttack_2) );

        _PassiveSkills.Clear();
        if (mySpecimen.passive_1 != null)
            _PassiveSkills.Add( PassiveSkill.AssignToCreature(gameObject, mySpecimen.passive_1) );
        if (mySpecimen.passive_2 != null)
            _PassiveSkills.Add( PassiveSkill.AssignToCreature(gameObject, mySpecimen.passive_2) );
        if (mySpecimen.passive_3 != null)
            _PassiveSkills.Add( PassiveSkill.AssignToCreature(gameObject, mySpecimen.passive_3) );

        SkillsChangedEvent?.Invoke();
    }

    //Add skill decisions to the specimen
    public void AddSkillsToLearn(int newLevel)
    {
        Specimen mySpecimen = GetComponent<CreatureStats>().GetSpecimen();

        for (int i = 0; i < mySpecimen.species.skillList.Length; i++)
        {
            if (mySpecimen.species.skillList[i].level == newLevel)
            {
                BaseSkill newSkill = mySpecimen.species.skillList[i].skill;
                //Flag signaling if there were free skill slots and the skill has been learnt directly with no need of player interaction
                bool learntDirectly = TryToLearnDirectly(mySpecimen, newSkill);

                if (learntDirectly == false)
                    mySpecimen.skillsToLearn.Add(mySpecimen.species.skillList[i].skill);
                else
                {
                    SkillsChangedEvent?.Invoke();
                }
            }
        }
    }

    // Checks if there is a free skill slot and the skill can be placed on it directly, without consulting the player
    private bool TryToLearnDirectly(Specimen mySpecimen, BaseSkill newSkill)
    {
        bool learntDirectly = false;

        if (newSkill is BaseActiveSkill)
        {   //Learn active skill
            BaseActiveSkill newActive = (BaseActiveSkill)newSkill;
            switch (newActive.category)
            {
                case SkillCategory.BASIC:
                    if (mySpecimen.basicAttack == null)
                    {
                        mySpecimen.basicAttack = newActive;
                        Skill.AssignToCreature(gameObject, newActive);
                        learntDirectly = true;
                    }
                    break;

                case SkillCategory.COOLDOWN:
                    if (mySpecimen.cooldownAttack_1 == null)
                    {
                        mySpecimen.cooldownAttack_1 = newActive;
                        Skill.AssignToCreature(gameObject, newActive);
                        learntDirectly = true;
                    }
                    else if (mySpecimen.cooldownAttack_2 == null)
                    {
                        mySpecimen.cooldownAttack_2 = newActive;
                        Skill.AssignToCreature(gameObject, newActive);
                        learntDirectly = true;
                    }
                    break;
            }

        }
        else if (newSkill is BasePassiveSkill)
        {   //Learn passive skill
            BasePassiveSkill newPassive = (BasePassiveSkill)newSkill;
            if (mySpecimen.passive_1 == null)
            {
                mySpecimen.passive_1 = newPassive;
                PassiveSkill.AssignToCreature(gameObject, newPassive);
                learntDirectly = true;
            }
            else if (mySpecimen.passive_2 == null)
            {
                mySpecimen.passive_2 = newPassive;
                PassiveSkill.AssignToCreature(gameObject, newPassive);
                learntDirectly = true;
            }
            else if (mySpecimen.passive_3 == null)
            {
                mySpecimen.passive_3 = newPassive;
                PassiveSkill.AssignToCreature(gameObject, newPassive);
                learntDirectly = true;
            }
        }

        return learntDirectly;
    }

    //Through this function the gui notifies the player decision over a new skill
    public void SkillLearnDecision(List<BaseSkill> skills, BaseSkill skillThatWasAcceptedOrRejected)
    {
        print("Decision deliver");
        if (skills != null && skills.Count > 0 && skills[0] != null)
        {
            Specimen mySpecimen = GetComponent<CreatureStats>().GetSpecimen();

            mySpecimen.RemoveSkillFromTheSkillsToLearnList(skillThatWasAcceptedOrRejected);

            if (skills[0] is BaseActiveSkill)
            {
                //Re-type the list of skills
                List<BaseActiveSkill> activeSkills = ReTypeSkills<BaseActiveSkill>(skills);

                ChangeActiveSkillInGameObject(activeSkills);

                SkillsChangedEvent?.Invoke();

                ChangeActiveSkillsInSpecimenDataStructure(mySpecimen, activeSkills);
            }

            else if (skills[0] is BasePassiveSkill)
            {
                List<BasePassiveSkill> pasiveSkills = ReTypeSkills<BasePassiveSkill>(skills);

                ChangePassiveSkillInGameObject(pasiveSkills);

                //Change the skills in the specimen data structure
                mySpecimen.passive_1 = pasiveSkills[0];
                mySpecimen.passive_2 = pasiveSkills[1];
                mySpecimen.passive_3 = pasiveSkills[2];
            }

            RefreshSkillReferences();
        }
    }



    private void ChangePassiveSkillInGameObject(List<BasePassiveSkill> pasiveSkills)
    {
        //Remove
        PassiveSkill[] currentSkills = GetComponents<PassiveSkill>();
        foreach (PassiveSkill skill in currentSkills)
        {
            skill.SetDestroyed();
            Destroy(skill);
        }
        //Add
        foreach (BasePassiveSkill skill in pasiveSkills)
            PassiveSkill.AssignToCreature(gameObject, skill);
    }

    private void ChangeActiveSkillInGameObject(List<BaseActiveSkill> activeSkills)
    {
        //Remove
        Skill[] currentSkills = GetComponents<Skill>();
        foreach (Skill skill in currentSkills)
            if (skill.category == activeSkills[0].category)
            {
                skill.SetDestroyed();
                Destroy(skill);
            }
        //Add
        foreach (BaseActiveSkill skill in activeSkills)
            Skill.AssignToCreature(gameObject, skill);
    }

    private void RefreshSkillReferences()
    {
        _CooldownSkills.Clear();

        Skill[] currentSkills = GetComponents<Skill>();
        foreach (Skill skill in currentSkills)
        {
            if (skill._IsDestroyed == false)
            {
                if (skill.category == SkillCategory.BASIC)
                    _BasicSkill = skill;
                else if (skill.category == SkillCategory.COOLDOWN)
                    _CooldownSkills.Add(skill);
            }
        }
    }

    private static void ChangeActiveSkillsInSpecimenDataStructure(Specimen mySpecimen, List<BaseActiveSkill> activeSkills)
    {
        //Change the skills in the specimen data structure
        switch (activeSkills[0].category)
        {
            case SkillCategory.BASIC:
                mySpecimen.basicAttack = activeSkills[0];
                break;

            case SkillCategory.COOLDOWN:
                mySpecimen.cooldownAttack_1 = activeSkills[0];
                mySpecimen.cooldownAttack_2 = activeSkills[1];
                break;

            case SkillCategory.ULTIMATE:
                mySpecimen.ultimateAttack = activeSkills[0];
                break;
        }
    }

    private static List<T> ReTypeSkills<T>(List<BaseSkill> skills) where T : BaseSkill
    {
        List<T> pasiveSkills = new List<T>();
        foreach (BaseSkill skill in skills)
            pasiveSkills.Add(skill as T);
        return pasiveSkills;
    }

    public Skill GetBasicSkill()
    {
        return _BasicSkill;
    }

    public List<Skill> GetCooldownSkills()
    {
        return _CooldownSkills;
    }
}
