using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSingleTarget : Skill {

    public override void StartExecution()
    {
    }

    public override void StartExecution(Vector3 targetArea)
    {
    }

    public override void StartExecution(GameObject target)
    {
        if (IsWithinRange(target) == false)
        {
            return;
        }
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, transform.position, transform.rotation, transform);
        targetCreature = target;
        ExecutionCounters();
    }

    public override void Execute()
    {   
        if (ValidTarget())
            targetCreature.SendMessage("Hit", GetSkillHit());
    }
}
