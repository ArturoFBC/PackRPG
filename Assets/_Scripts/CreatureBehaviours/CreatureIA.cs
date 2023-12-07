using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum IA_Mode
{
    AGGRESIVE,
    DEFENSIVE,
    PASSIVE
}

[RequireComponent(typeof(CreatureIABasic))]
public class CreatureIA : MonoBehaviour
{
    public float _FollowDistance = 10f;
    public float _AutoAttackDistance = 10f;

    private int _Index;

    //On target assign, the AI should subscribe to a death event, so if the target dies the player would stop and reevaluate his behaviour
    //public GameObject _Target;

    public CreatureIABasic _MyBasicIA;
    public CreatureSkillManager _MySkillManager;

	void Awake ()
    {
        if (_MyBasicIA == null)
            _MyBasicIA = GetComponent<CreatureIABasic>();

        if (_MySkillManager == null)
            _MySkillManager = GetComponent<CreatureSkillManager>();
    }

    private void Start()
    {
        for (int i = 0; i < GameManager.playerCreatures.Count; i++)
        {
            if ( GameManager.playerCreatures[i] == gameObject )
            {
                _Index = i;
                break;
            }
        }
    }

    void Update ()
    {
        if (_MyBasicIA._Status == BasicIAStatus.IDLE)
        {
            //Take automatic actions only if the control mode is right and the selected creature is not this
            if (PlayerInput.Ref.controlMode == ControlMode.FOLLOW &&
                GameManager.selectedCreature != null &&
                GameManager.selectedCreature != gameObject)
            {   //Decide wether to follow the selected creature (follow has highter priority than auto attack)
                if ((transform.position - GameManager.selectedCreature.transform.position).magnitude > _FollowDistance)
                {
                    Vector3 moveDirection = (GameManager.selectedCreature.transform.position - transform.position).normalized;
                    Vector3 normal = Vector3.Cross(moveDirection, Vector3.up);
                    Vector3 destination = GameManager.selectedCreature.transform.position - moveDirection + (normal * (_Index - 1.5f) * 1.5f);
                    _MyBasicIA.MoveCommand(destination);
                }
                else
                {
                    bool usingSkill = false; //Flag to mark wether we found a skill to use
                    //Look for skills to use
                    foreach (Skill s in _MySkillManager.GetCooldownSkills())
                    {
                        if (s.isUsable)
                        {
                            GameObject target = s.GetTargets();
                            if (target != null)
                            {
                                if (s._CurrentSkillInfo.targetType == TargetType.SELF)
                                    _MyBasicIA.UseSkill(_MySkillManager.GetCooldownSkills().IndexOf(s));
                                else
                                    _MyBasicIA.AttackCommand(target, _MySkillManager.GetCooldownSkills().IndexOf(s));

                                usingSkill = true;
                                break;
                            }
                        }
                    }

                    //Auto attack only if we are not already using a skill
                    if (usingSkill == false)
                    {//Look for targets to auto-attack
                        Collider[] posibleTargets = Physics.OverlapSphere(transform.position, _AutoAttackDistance);
                        foreach (Collider c in posibleTargets)
                        {
                            if ((gameObject.tag == "Enemy" && c.tag == "Player") ||
                                 (gameObject.tag == "Player" && c.tag == "Enemy"))
                            {
                                _MyBasicIA.AttackCommand(c.gameObject);
                                break;
                            }
                        }
                    }
                }
            }
        }
	}
}
