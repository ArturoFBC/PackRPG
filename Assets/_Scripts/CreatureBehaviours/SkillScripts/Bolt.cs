using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : Skill
{

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
        {
            GameObject effect = Instantiate(_CurrentSkillInfo.useVFX, transform.position, transform.rotation, transform);

            LightningBoltEffect lightningBoltEffect = effect.GetComponent<LightningBoltEffect>();
            if ( lightningBoltEffect != null )
            {
                Vector3 startPoint = Physics.ClosestPoint(target.transform.position + (Vector3.up * 2), GetComponent<Collider>(), transform.position, transform.rotation);
                Vector3 endPoint = Physics.ClosestPoint(transform.position + (Vector3.up * 2), target.GetComponent<Collider>(), target.transform.position, target.transform.rotation);

                lightningBoltEffect.Set(startPoint, endPoint);
                lightningBoltEffect.enabled = true;
            }
        }
        targetCreature = target;
        ExecutionCounters();
    }

    public override void Execute()
    {
        if (ValidTarget())
            targetCreature.SendMessage("Hit", GetSkillHit());
    }
}
