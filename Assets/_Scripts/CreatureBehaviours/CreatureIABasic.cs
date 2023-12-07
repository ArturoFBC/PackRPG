using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BasicIAStatus
{
    ATTACKING,
    MOVING,
    IDLE,
    DEAD
}

public enum MovementTarget
{
    CREATURE,
    POSITION,
    NONE
}

public enum NextAction
{
    ATTACK,
    MOVE,
    INTERACT,
    NONE
}

public class CreatureIABasic : MonoBehaviour
{

    public bool _AutoAttack;

    public BasicIAStatus _Status;
    private NextAction _NextAction = NextAction.NONE;

    public CreatureMovement _MyPlayerMovement;

    public GameObject _BasicAttackTarget;
    public GameObject _SkillTargetCreature;
    public Vector3 _TargetPosition;
    public MovementTarget _TargetType;

    public CreatureSkillManager _MySkillManager;

    public Skill _CurrentSkill;

    public static float _InteractRange = 3f;

    void Awake()
    {
        if (_MyPlayerMovement == null)
            _MyPlayerMovement = GetComponent<CreatureMovement>();

        if (_MyPlayerMovement != null)
        {
            _MyPlayerMovement.MoveEndEvent -= MoveEnded;
            _MyPlayerMovement.MoveEndEvent += MoveEnded;
        }
    }

    private void Start()
    {
        //Subscribe to knockout and arise
        CreatureHitPoints myCreatureLife = GetComponent<CreatureHitPoints>();
        myCreatureLife.KnockOutEvent -= OnDeath;
        myCreatureLife.KnockOutEvent += OnDeath;
        myCreatureLife.AriseEvent -= OnRise;
        myCreatureLife.AriseEvent += OnRise;

        //Subscrible to changes in skills
        if (_MySkillManager == null)
            _MySkillManager = GetComponent<CreatureSkillManager>();
    }

    public void MoveCommand(Vector3 destination)
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            _TargetPosition = destination;
            _BasicAttackTarget = null;
            if (_Status == BasicIAStatus.IDLE || _Status == BasicIAStatus.MOVING)
            {
                StartMovement(_TargetPosition, false);
                _NextAction = NextAction.NONE;
                _Status = BasicIAStatus.MOVING;
            }
            else
                _NextAction = NextAction.MOVE;
        }
    }

    public void StartMovement(Vector3 destination, bool useRange)
    {
        if (useRange)
            _MyPlayerMovement.MoveTo(destination, _CurrentSkill._CurrentSkillInfo.useRange);
        else
            _MyPlayerMovement.MoveTo(destination);
    }

    public void StartMovement(GameObject target, bool useRange)
    {
        if (useRange)
            _MyPlayerMovement.MoveTo(target, _CurrentSkill._CurrentSkillInfo.useRange);
        else
            _MyPlayerMovement.MoveTo(target);
    }

    public void MoveEnded()
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            if (_NextAction == NextAction.NONE)
            {
                _Status = BasicIAStatus.IDLE;
            }
            else if (_NextAction == NextAction.ATTACK)
            {
                if (_TargetType == MovementTarget.CREATURE &&
                   ((_CurrentSkill == _MySkillManager.GetBasicSkill() && _BasicAttackTarget == null) ||
                     (_CurrentSkill != _MySkillManager.GetBasicSkill() && _SkillTargetCreature == null)))
                {
                    //Target lost
                    _Status = BasicIAStatus.IDLE;
                }
                else
                {
                    MoveInToUseSkill();
                }
            }
            else if ( _NextAction == NextAction.INTERACT)
            {
                if (_BasicAttackTarget == null)
                    _Status = BasicIAStatus.IDLE;
                else
                    MoveInToInteract();
            }
        }
    }

    public void AttackCommand(GameObject target, int skillIndex = -1)
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            if (skillIndex == -1)
                _BasicAttackTarget = target;
            else
                _SkillTargetCreature = target;
            _TargetType = MovementTarget.CREATURE;

            Attack(skillIndex);
        }
    }

    public void AttackCommand(Vector3 target, int skillIndex = -1)
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            _TargetPosition = target;
            _TargetType = MovementTarget.POSITION;

            Attack(skillIndex);
        }
    }

    public void UseSkill(int skillIndex)
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            _TargetType = MovementTarget.NONE;
            Attack(skillIndex);
        }
    }

    private void Attack(int skillIndex)
    {
        if (skillIndex < 0)
            _CurrentSkill = _MySkillManager.GetBasicSkill();
        else
            _CurrentSkill = _MySkillManager.GetCooldownSkills()[skillIndex];

        switch (_Status)
        {
            case BasicIAStatus.IDLE:
                MoveInToUseSkill();
                break;
            case BasicIAStatus.MOVING:
                _NextAction = NextAction.ATTACK;
                _MyPlayerMovement.AbortMovement();
                break;
            case BasicIAStatus.ATTACKING:
                MoveInToUseSkill();
                break;
        }
    }

    public void InteractCommand(GameObject target)
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            _BasicAttackTarget = target;
            _NextAction = NextAction.INTERACT;

            switch (_Status)
            {
                case BasicIAStatus.IDLE:
                    MoveInToInteract();
                    break;
                case BasicIAStatus.MOVING:
                    _MyPlayerMovement.AbortMovement();
                    MoveInToInteract();
                    break;
                case BasicIAStatus.ATTACKING:
                    MoveInToInteract();
                    break;
            }
        }
    }

    private void MoveInToInteract()
    {
        bool inRange = (_BasicAttackTarget.transform.position - transform.position).sqrMagnitude < Mathf.Pow(_InteractRange, 2);
        if (inRange)
            Interact();
        else
        {
            _Status = BasicIAStatus.MOVING;
            _MyPlayerMovement.MoveTo(_BasicAttackTarget, _InteractRange);
        }
    }

    private void Interact()
    {
        _BasicAttackTarget.SendMessage("Interact", transform);
        _NextAction = NextAction.NONE;
        _Status = BasicIAStatus.IDLE;
    }

    private void MoveInToUseSkill()
    {
        bool inRange = true;
        if (_TargetType == MovementTarget.CREATURE)
        {
            if (_CurrentSkill == _MySkillManager.GetBasicSkill())
                inRange = _CurrentSkill.IsWithinRange(_BasicAttackTarget);
            else
                inRange = _CurrentSkill.IsWithinRange(_SkillTargetCreature);
        }
        else if (_TargetType == MovementTarget.POSITION)
            inRange = _CurrentSkill.IsWithinRange(_TargetPosition);
        else if (_TargetType == MovementTarget.NONE)
            inRange = true;

        if (inRange)
        {
            _Status = BasicIAStatus.IDLE;
            UseSkill();
        }
        else
        {
            Vector3 destination = _TargetPosition;
            if (_TargetType == MovementTarget.CREATURE)
            {
                if (_CurrentSkill == _MySkillManager.GetBasicSkill())
                    StartMovement(_BasicAttackTarget, true);
                else
                    StartMovement(_SkillTargetCreature, true);
            }
            else
                StartMovement(destination, true);

            _Status = BasicIAStatus.MOVING;
            _NextAction = NextAction.ATTACK;
        }
    }

    private void UseSkill()
    {
        if (_CurrentSkill.isUsable)
        {
            switch (_CurrentSkill.targetType)
            {
                case TargetType.GROUND:
                    if (_TargetType == MovementTarget.POSITION)
                    {
                        _MyPlayerMovement.LookAt(_TargetPosition);
                        _CurrentSkill.StartExecution(_TargetPosition);
                    }
                    else
                    {
                        if (_CurrentSkill == _MySkillManager.GetBasicSkill())
                        {
                            _MyPlayerMovement.LookAt(_BasicAttackTarget.transform.position);
                            _CurrentSkill.StartExecution(_BasicAttackTarget.transform.position);
                        }
                        else
                        {
                            _MyPlayerMovement.LookAt(_SkillTargetCreature.transform.position);
                            _CurrentSkill.StartExecution(_SkillTargetCreature.transform.position);
                        }
                    }

                    break;

                case TargetType.SELF:
                    _CurrentSkill.StartExecution();
                    break;

                default:
                    if (_CurrentSkill == _MySkillManager.GetBasicSkill())
                    {
                        ;
                        _MyPlayerMovement.LookAt(_BasicAttackTarget.transform.position);
                        _CurrentSkill.StartExecution(_BasicAttackTarget);
                    }
                    else
                    {
                        _MyPlayerMovement.LookAt(_SkillTargetCreature.transform.position);
                        _CurrentSkill.StartExecution(_SkillTargetCreature);
                    }
                    break;
            }
            _Status = BasicIAStatus.ATTACKING;
            _NextAction = NextAction.NONE;
        }
    }

    public void SkillEnded()
    {
        if (_Status != BasicIAStatus.DEAD)
        {
            if (_NextAction == NextAction.MOVE)
            {
                _NextAction = NextAction.NONE;
                _Status = BasicIAStatus.MOVING;
                StartMovement(_TargetPosition, false);
            }
            else if (_AutoAttack && _BasicAttackTarget != null && _NextAction != NextAction.MOVE)
            {
                _CurrentSkill = _MySkillManager.GetBasicSkill();
                _Status = BasicIAStatus.IDLE;
                _NextAction = NextAction.ATTACK;
                MoveInToUseSkill();
            }
            else
                _Status = BasicIAStatus.IDLE;
        }
    }

    public void Update()
    {
        //Resume auto-attack
        if ( _Status == BasicIAStatus.IDLE && _NextAction != NextAction.NONE && _AutoAttack && _BasicAttackTarget != null )
        {
            MoveInToUseSkill();
        }
    }

    public void OnDeath()
    {
        _Status = BasicIAStatus.DEAD;
        _NextAction = NextAction.NONE;
        _MyPlayerMovement.AbortMovement();
    }

    public void OnRise()
    {
        _Status = BasicIAStatus.IDLE;
    }
}
