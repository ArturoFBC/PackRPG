using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIA : MonoBehaviour {

    [SerializeField] private CreatureIABasic _MyBasicIA;
    [SerializeField] private RoamingBehabiour _MyRoamingBehaviour;

    private List<GameObject> _KnownTargets = new List<GameObject>();
    private GameObject _CurrentTarget;

    private List<Skill> _MySkills = new List<Skill>();

    private Vector3 _StartingLocation;
    [SerializeField] private float _MaxPursuitDistance = 40f;

    private void Awake()
    {
        if (_MyBasicIA == null)
            _MyBasicIA = GetComponent<CreatureIABasic>();

        CreatureHitPoints myHitPoints = GetComponent<CreatureHitPoints>();
        if (myHitPoints != null)
            myHitPoints.HitReceivedEvent += OnHit;
    }

    void Update ()
    {
        if (_StartingLocation == Vector3.zero)
        {
            _MyRoamingBehaviour = gameObject.AddComponent<RoamingBehabiour>();
            _MyRoamingBehaviour.enabled = true;
            _StartingLocation = transform.position;
        }

        if ( _MyBasicIA._Status == BasicIAStatus.IDLE && _CurrentTarget != null )
        {
            if (_CurrentTarget.activeInHierarchy)
                _MyBasicIA.AttackCommand(_CurrentTarget);
            else
                ForgetTarget();
        }
        else if ( Vector3.SqrMagnitude(transform.position - _StartingLocation) > ( _MaxPursuitDistance * _MaxPursuitDistance ) )
        {
            _MyBasicIA.MoveCommand(_StartingLocation);
        }
        else if (_MyBasicIA._Status == BasicIAStatus.IDLE && _CurrentTarget == null)
        {
            _MyRoamingBehaviour.enabled = true;
        }
    }

#region INFO_RECEIVED_FROM_EXTERNAL_SOURCES_ABOUT_TARGETS
    public void TargetDetected( GameObject target )
    {
        if (_CurrentTarget == null)
            AcquireTarget(target);

        if (_KnownTargets.Contains(target) == false)
            _KnownTargets.Add(target);
    }

    public void TargetLost( GameObject target )
    {
        if (_KnownTargets.Contains(target))
            _KnownTargets.Remove(target);

        if (_CurrentTarget != null && target == _CurrentTarget)
            ForgetTarget();

        Retarget();
    }

    public void OnHit(SkillHit hit)
    {
        if (hit.attacker != null)
            TargetDetected(hit.attacker);
    }

    public void OnProvoked(GameObject target)
    {
        AcquireTarget( target );
    }
    #endregion

    #region INTERNAL_MANAGEMENT_OF_TARGETS
    private void Retarget()
    {
        if (_CurrentTarget == null && _KnownTargets.Count > 0)
        {
            List<GameObject> cleanList = new List<GameObject>( _KnownTargets );
            cleanList.RemoveAll(item => item == null);
            _KnownTargets = cleanList;

            for ( int i = 0; i < _KnownTargets.Count; i++ )
            {
                if (_KnownTargets[i].GetComponent<CreatureHitPoints>() != null)
                {
                    AcquireTarget(_KnownTargets[i]);
                    break;
                }
            }
        }
    }

    private void AcquireTarget( GameObject target )
    {
        if (target != null)
        {
            _CurrentTarget = target;

            CreatureHitPoints targetLife = target.GetComponent<CreatureHitPoints>();
            if (targetLife != null)
            {
                targetLife.KnockOutEvent -= OnTargetKOed;
                targetLife.KnockOutEvent += OnTargetKOed;
            }

            _MyRoamingBehaviour.enabled = false;
            _MyBasicIA.AttackCommand(target);
        }
    }

    private void ForgetTarget()
    {
        CreatureHitPoints targetLife = _CurrentTarget.GetComponent<CreatureHitPoints>();
        if (targetLife != null)
            targetLife.KnockOutEvent -= OnTargetKOed;

        _CurrentTarget = null;
    }

    public void OnTargetKOed()
    {
        TargetLost(_CurrentTarget);
    }
    #endregion

    public void OnDestroy()
    {
        CreatureHitPoints myHitPoints = GetComponent<CreatureHitPoints>();
        if (myHitPoints != null)
            myHitPoints.HitReceivedEvent -= OnHit;
    }
}
