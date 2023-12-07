using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureFXandAnimation : MonoBehaviour
{
    private CreatureStats _MyCreatureStats;

    public Animator _MyAnimator;
    private float _BaseAnimationSpeed;

    public NavMeshAgent _NavMeshAgent;
    private float _BaseMoveSpeed;

    private GameObject _BurnEffect;
    private GameObject _FreezeEffect;
    private GameObject _FreezeMesh;
    private GameObject _ShockEffect;

    private void Start()
    {
        if (_MyAnimator == null)
            _MyAnimator = GetComponentInChildren<Animator>();

        _BaseAnimationSpeed = _MyAnimator.speed;

        if (_NavMeshAgent == null)
            _NavMeshAgent = GetComponentInChildren<NavMeshAgent>();

        _BaseMoveSpeed = _NavMeshAgent.speed;

        if (_MyCreatureStats == null)
            _MyCreatureStats = GetComponentInChildren<CreatureStats>();

        if (_MyCreatureStats)
        {
            SetAnimationSpeed(_MyCreatureStats.GetStat(PrimaryStat.ANIMATION_SPEED));
            SetMoveSpeed(_MyCreatureStats.GetStat(PrimaryStat.MOVE_SPEED));
        }
    }

    private void OnEnable()
    {
        LinkEvents();
    }

    private void OnDisable()
    {
        UnlinkEvents();
    }

    private void LinkEvents()
    {
        if (_MyCreatureStats != null)
        {
            _MyCreatureStats._StatEvents[(int)PrimaryStat.ANIMATION_SPEED] += SetAnimationSpeed;
            _MyCreatureStats._StatEvents[(int)PrimaryStat.MOVE_SPEED]      += SetMoveSpeed;
        }
    }

    private void UnlinkEvents()
    {
        if (_MyCreatureStats != null)
        {
            _MyCreatureStats._StatEvents[(int)PrimaryStat.ANIMATION_SPEED] -= SetAnimationSpeed;
            _MyCreatureStats._StatEvents[(int)PrimaryStat.MOVE_SPEED]      -= SetMoveSpeed;
        }
    }

    public void SetAnimationSpeed(float modifier)
    {
        _MyAnimator.speed = _BaseAnimationSpeed * modifier;
    }

    public void SetMoveSpeed(float modifier)
    {
        _NavMeshAgent.speed = _BaseMoveSpeed * modifier;
    }

    public void SetEffect( StatusEffectType status, bool active = true)
    {
        switch (status)
        {
            case StatusEffectType.BURNT:
                if (active)
                {
                    if (_BurnEffect == null)
                        _BurnEffect = Instantiate(IconsAndEffects._Ref.BurnEffect, transform);
                    else
                        _BurnEffect.SetActive(true);
                }
                else
                {
                    if (_BurnEffect != null)
                        _BurnEffect.SetActive(false);
                }
            break;
            case StatusEffectType.ELECTRIFIED:
                if (active)
                {
                    if (_ShockEffect == null)
                        _ShockEffect = Instantiate(IconsAndEffects._Ref.ShockEffect, transform);
                    else
                        _ShockEffect.SetActive(true);
                }
                else
                {
                    if (_ShockEffect != null)
                        _ShockEffect.SetActive(false);
                }
            break;
            case StatusEffectType.FROZEN:
                if (active)
                {
                    if (_FreezeEffect == null)
                        _FreezeEffect = Instantiate(IconsAndEffects._Ref.FreezeEffect, transform);
                    else
                        _FreezeEffect.SetActive(true);

                    if (_FreezeMesh == null)
                    {
                        foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
                        {
                            SkinnedMeshRenderer newSMR = Instantiate(smr.gameObject, smr.transform.parent).GetComponent<SkinnedMeshRenderer>();
                            _FreezeMesh = newSMR.gameObject;
                            newSMR.material = IconsAndEffects._Ref.FreezeMaterial;
                        }
                    }
                    else
                        _FreezeMesh.SetActive(true);

                    SetAnimationSpeed(0.5f);
                    SetMoveSpeed(0.5f);
                }
                else
                {
                    if (_FreezeEffect != null)
                        _FreezeEffect.SetActive(false);

                    if (_FreezeMesh != null)
                        _FreezeMesh.SetActive(false);

                    SetAnimationSpeed(1f);
                    SetMoveSpeed(1f);
                }
            break;
        }
    }
}
