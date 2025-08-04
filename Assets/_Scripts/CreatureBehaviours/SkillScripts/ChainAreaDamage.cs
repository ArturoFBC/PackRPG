using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainAreaDamage : Skill
{
    [SerializeField] private float chainDistance = 4f;
    [SerializeField] private int chainAmount = 3;
    [SerializeField] private float chainDelay = 0.2f;

    [SerializeField] private float lighitningHeight = 15f;

    public override void Execute()
    {
        _MyMana.ConsumeMana(_CurrentSkillInfo.manaCost);

        Vector3 offset = transform.forward.normalized * chainDistance;

        StartCoroutine( CallLigningCoroutine(offset));
    }

    private IEnumerator CallLigningCoroutine(Vector3 offset)
    {
        for (int hitIndex = 0; hitIndex < chainAmount; hitIndex++)
        {
            Vector3 hitPosition = transform.position + offset * (hitIndex + 1);

            DisplayVFX(hitPosition);

            Collider[] posibleTargets = Physics.OverlapSphere(hitPosition, _CurrentSkillInfo.effectRange);

            SkillHit hit = GetSkillHit();
            foreach (Collider c in posibleTargets)
            {
                if (c.tag == "Enemy" && Vector3.Angle(transform.forward, c.transform.position - transform.position) < 30)
                {
                    c.SendMessage("Hit", hit);
                }
            }

            yield return new WaitForSeconds(chainDelay);
        }
    }

    private void DisplayVFX(Vector3 hitPosition)
    {
        if (_CurrentSkillInfo.useVFX != null)
        {
            Transform FXTransform = Instantiate(_CurrentSkillInfo.useVFX, hitPosition, Quaternion.identity).transform;

            LightningBoltEffect lightningBoltEffect = FXTransform.gameObject.GetComponent<LightningBoltEffect>();
            if (lightningBoltEffect != null)
            {
                lightningBoltEffect.Set(hitPosition + (Vector3.up * lighitningHeight), hitPosition);
                lightningBoltEffect.enabled = true;
            }
        }
    }

    public override void StartExecution()
    {
    }

    public override void StartExecution(Vector3 targetArea)
    {
        _MyAnimator.SetTrigger(_CurrentSkillInfo.animationTrigger);

        ExecutionCounters();
    }

    public override void StartExecution(GameObject target)
    {
        StartExecution(target.transform.position);
    }
}
