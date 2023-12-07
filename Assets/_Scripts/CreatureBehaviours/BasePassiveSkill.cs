using UnityEngine;

public enum PassiveImplementation
{
    EffectOnCondition,
    PassiveAreaDamage,
    PassiveAreaHealing,
    END
}

[CreateAssetMenu(menuName = "Skills/BasePassiveSkill")]
public class BasePassiveSkill:BaseSkill
{
    public PassiveImplementation impelementation;
}
