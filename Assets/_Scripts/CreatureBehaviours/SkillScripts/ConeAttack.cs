using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeAttack : Skill {

    public override void StartExecution()
    {
    }


    public override void StartExecution(Vector3 targetArea)
    {
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
        {
            Transform FXTransform = Instantiate(_CurrentSkillInfo.useVFX, transform.position, transform.rotation).transform;
            //FXTransform.LookAt(targetArea);
            FXTransform.position += new Vector3(0, 1f, 0);
        }


        ExecutionCounters();
    }

    public override void StartExecution(GameObject target)
    {
        StartExecution(target.transform.position);
    }

    public override void Execute()
    {
        _MyMana.ConsumeMana(_CurrentSkillInfo.manaCost);

        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _CurrentSkillInfo.effectRange);

        SkillHit hit = GetSkillHit();
        foreach (Collider c in posibleTargets)
        {
            if (c.tag == "Enemy" && Vector3.Angle(transform.forward, c.transform.position - transform.position) < 30)
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
                 (gameObject.tag == "Player" && c.tag == "Enemy") )
            {
                return c.gameObject;
            }
        }
        return null;
    }
}
