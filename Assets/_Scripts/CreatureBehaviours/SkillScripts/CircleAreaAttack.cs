using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleAreaAttack : Skill
{
    public override void StartExecution()
    {
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, transform.position + new Vector3(0f,1f,0f), transform.rotation);
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

        SkillHit hit = GetSkillHit();
        foreach (Collider c in posibleTargets)
        {
            if ( ( gameObject.tag == "Enemy" && c.tag == "Player" ) ||
                 ( gameObject.tag == "Player" && c.tag == "Enemy" ) )
            {
                c.SendMessage("Hit", hit);
            }
        }
    }

    public override GameObject GetTargets()
    {
        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _CurrentSkillInfo.effectRange);
        foreach (Collider c in posibleTargets)
        {
            if ((gameObject.tag == "Enemy" && c.tag == "Player") ||
                 (gameObject.tag == "Player" && c.tag == "Enemy"))
            {
                return c.gameObject;
            }
        }
        return null;
    }
}
