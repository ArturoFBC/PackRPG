using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/SkillList")]
public class ScriptableSkillList : ScriptableObject
{
    public List<BaseSkill> skills;
}
