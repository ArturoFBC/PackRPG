using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class CreatureMovement : MonoBehaviour {

    [SerializeField]
    private NavMeshAgent _MyNavigationAgent;
    [SerializeField]
    private Animator _MyAnimator;

    public delegate void MoveStart(Vector3 target);
    public event MoveStart MoveStartEvent;
    public delegate void MoveEnd();
    public event MoveEnd MoveEndEvent;

    GameObject _Target;
    Vector3 _Destination;
    MovementTarget _TargetType = MovementTarget.POSITION;

    float _Range;

    public bool _WaitingForPath;

    bool _moving => _MyNavigationAgent.hasPath;

	void Start ()
    {
        if (_MyNavigationAgent == null)
            _MyNavigationAgent = GetComponent<NavMeshAgent>();

        if (_MyAnimator == null)
            _MyAnimator = GetComponentInChildren<Animator>();

        MoveEndEvent += OnMovementEnd;
        MoveStartEvent += OnMovementStart;
    }

    private void Update()
    {
        if (_WaitingForPath == true)
        {
            if (_MyNavigationAgent.hasPath)
            {
                _WaitingForPath = false;
                MoveStartEvent?.Invoke(_MyNavigationAgent.destination);
            }
        }
        else
        {//Check if we are close enough to the target position
            bool withinRange = IsWhitinDistanceOfTarget();
            if (_MyAnimator != null && _MyAnimator.GetBool("Walk"))
            {
                if (withinRange)
                {
                    _MyNavigationAgent.ResetPath();
                    MoveEndEvent?.Invoke();
                }
                //Check if the target creature has moved enough to make us repath
                else if (_TargetType == MovementTarget.CREATURE)
                {
                    if (_Target == null)
                    {
                        MoveEndEvent?.Invoke();
                    }
                    else if ((_Target.transform.position - _Destination).sqrMagnitude > (_Range * _Range))
                    {
                        _Destination = _Target.transform.position;
                        _WaitingForPath = _MyNavigationAgent.SetDestination(_Destination);
                    }
                }
            }
        }
    }

    public void LookAt( Vector3 position )
    {
        _MyNavigationAgent.updateRotation = false;
        Vector3 targetPostition = new Vector3(position.x,
                                       this.transform.position.y,
                                       position.z);
        this.transform.LookAt(targetPostition);
    }

    public void MoveTo(Vector3 seTdestination, float setRange = 1f, bool movingTarget = false)
    {
        if (movingTarget)
            _TargetType = MovementTarget.CREATURE;
        else
            _TargetType = MovementTarget.POSITION;

        _WaitingForPath = _MyNavigationAgent.SetDestination(seTdestination);
        _Range = setRange;
        _Destination = seTdestination;
        _MyNavigationAgent.updateRotation = true;
    }

    public void MoveTo(GameObject gameObjectTarget, float setRange = 1f)
    {
        _Target = gameObjectTarget;
        MoveTo(gameObjectTarget.transform.position, setRange, true);
    }

    public void AbortMovement()
    {
        if (_MyNavigationAgent.isOnNavMesh)
            _MyNavigationAgent.ResetPath();

        MoveEndEvent?.Invoke();
    }

    private void OnMovementStart( Vector3 destination )
    {
        _MyAnimator.SetBool("Walk", true);
    }

    private void OnMovementEnd()
    {
        _Target = null;
        _MyAnimator?.SetBool("Walk", false);
    }

    private bool IsWhitinDistanceOfTarget()
    {
        Vector3 targetPosition = _Destination;
        if ( _TargetType == MovementTarget.CREATURE && _Target != null )
            targetPosition = _Target.transform.position;

        return (_MyNavigationAgent.transform.position - targetPosition).sqrMagnitude < (_Range * _Range);
    }
}
