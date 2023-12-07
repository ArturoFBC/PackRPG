using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffectDamageOverTime : Skill {

    Vector3 _TargetGround;

    public override void StartExecution()
    {
    }

    public override void StartExecution(Vector3 targetArea)
    {
        _TargetGround = targetArea;
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);
        if (_CurrentSkillInfo.useVFX != null)
            Instantiate(_CurrentSkillInfo.useVFX, transform.position, transform.rotation, transform);
        ExecutionCounters();
    }

    public override void StartExecution(GameObject target)
    {
        StartExecution(target.transform.position);
    }

    public override void Execute()
    {
        _MyMana.ConsumeMana(_CurrentSkillInfo.manaCost);

        AreaDamageObject newProjectile = Instantiate(_CurrentSkillInfo.projectile, _TargetGround + new Vector3(0f, 1f, 0f), Quaternion.identity).GetComponent<AreaDamageObject>();

        string targetTag = (gameObject.tag == "Player" ? "Enemy" : "Player");
        newProjectile.Set(_CurrentSkillInfo.creationsDuration, GetSkillHit(), targetTag, _CurrentSkillInfo.effectRange);
    }
}
