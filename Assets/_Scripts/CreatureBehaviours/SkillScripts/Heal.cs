using UnityEngine;
using System.Collections;

public class Heal : Skill
{

    public override void StartExecution()
    {
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, transform.position + new Vector3(0f, 1f, 0f), transform.rotation, transform);

        targetCreature = gameObject;

        ExecutionCounters();
    }


    public override void StartExecution(Vector3 targetArea)
    {
    }

    public override void StartExecution(GameObject target)
    {
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, target.transform.position + new Vector3(0f, 1f, 0f), target.transform.rotation, target.transform);

        targetCreature = target;

        ExecutionCounters();
    }

    public override void Execute()
    {
        _MyMana.ConsumeMana(_CurrentSkillInfo.manaCost);

        if ( ValidTarget() )
            targetCreature.SendMessage( "Heal", GetHealInstance() );
    }

    public override GameObject GetTargets()
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _CurrentSkillInfo.useRange);

        //Find ally with lowest HP percentage
        GameObject possibleTarget = null;
        float healtPercent = 1f;
        foreach (Collider c in posibleTargets)
        {
            if ((gameObject.tag == "Enemy" && c.tag == "Enemy") ||
                 (gameObject.tag == "Player" && c.tag == "Player"))
            {
                CreatureHitPoints possibleTargetLife = c.GetComponent<CreatureHitPoints>();
                if (possibleTargetLife != null)
                {
                    float tempHealthPercent = possibleTargetLife.currentHP / possibleTargetLife.MaxHP;
                    if (tempHealthPercent < healtPercent)
                    {
                        possibleTarget = c.gameObject;
                        healtPercent = tempHealthPercent;
                    }
                }
            }
        }

        return healtPercent > 0.5f ? null : possibleTarget;
    } 
}
