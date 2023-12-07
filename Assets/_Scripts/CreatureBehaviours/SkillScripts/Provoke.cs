using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Provoke : Skill {

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
            if (c.tag == "Enemy")
            {
                c.GetComponent<CreatureIABasic>().AttackCommand( gameObject );
                c.GetComponent<EnemyIA>().OnProvoked( gameObject );
            }
        }
    }
}
