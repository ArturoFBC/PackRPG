using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityBuff : Skill {

    public override void StartExecution()
    {
        _MyAnimator.SetTrigger(_MyBaseSkill.animationTrigger);
        if (_MyBaseSkill.useVisualEffect != null)
            Instantiate(_MyBaseSkill.useVisualEffect, transform.position + new Vector3(0f, 1f, 0f), transform.rotation, transform);
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
        _MyMana.ConsumeMana(_MyBaseSkill.manaCost);

        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _MyBaseSkill.effectRange);

        SkillHit hit = GetSkillHit();
        foreach (Collider c in posibleTargets)
        {
            if (gameObject.tag == c.tag)
            {
                c.SendMessage("Hit", hit);
            }
        }
    }
}
