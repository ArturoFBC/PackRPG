using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaunch : Skill
{

    public override void StartExecution()
    {
    }

    public override void StartExecution(Vector3 targetArea)
    {
        if (IsWithinRange(targetArea) == false)
        {
            return;
        }
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, transform.position, transform.rotation, transform);
        ExecutionCounters();
    }

    public override void StartExecution(GameObject target)
    {
        targetCreature = target;
        StartExecution(target.transform.position);
    }

    public override void Execute()
    {
        PlayerProjectile newProjectile = Instantiate(_CurrentSkillInfo.projectile, transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity).GetComponent<PlayerProjectile>();
        newProjectile.transform.rotation = transform.rotation;
        newProjectile.Set(GetSkillHit());
    }
}
