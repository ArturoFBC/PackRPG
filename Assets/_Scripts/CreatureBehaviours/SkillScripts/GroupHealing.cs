using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupHealing : Skill
{
    public override void StartExecution()
    {
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, transform.position + new Vector3(0f, 1f, 0f), transform.rotation, transform);
        ExecutionCounters();
    }

    public override void StartExecution(Vector3 targetArea)
    {
    }

    public override void StartExecution(GameObject target)
    {
    }

    public override void Execute()
    {
        _MyMana.ConsumeMana(_CurrentSkillInfo.manaCost);

        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _CurrentSkillInfo.effectRange);

        foreach (Collider c in posibleTargets)
        {
            if (gameObject.tag == c.tag)
            {
                if (_CurrentSkillInfo.targetVFX != null)
                    Instantiate(_CurrentSkillInfo.targetVFX, c.transform);

                c.SendMessage("Heal", GetHealInstance());
            }
        }
    }

    public override GameObject GetTargets()
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _CurrentSkillInfo.effectRange);
        foreach (Collider c in posibleTargets)
        {
            if (gameObject.tag == c.tag)
            {
                CreatureHitPoints possibleTargetLife = c.GetComponent<CreatureHitPoints>();
                float healtPercent = possibleTargetLife.currentHP / possibleTargetLife.MaxHP;

                if ( healtPercent < 0.5f )
                    return c.gameObject;
            }
        }
        return null;
    }
}
