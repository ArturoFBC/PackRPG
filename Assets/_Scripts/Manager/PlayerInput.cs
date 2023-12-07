using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ControlMode
{
    //Commands are sent to all members of the party
    MIMIC,
    //Commands are sent to selected members only, but the rest follow if they are too far
    FOLLOW
}

public enum ActionInput
{
    SKILL,
    ITEM,
    NONE
}

public class PlayerInput : MonoBehaviour {

    static public PlayerInput Ref;

    [SerializeField]
    private ControlMode _ControlMode;
    public ControlMode controlMode
    {
        get
        {
            return this._ControlMode;
        }
    }

    //Skill usage
    private ActionInput _ActionSelected = ActionInput.NONE;
    private int _CreatureIndex;
    private int _SkillIndex;

    public delegate void SkillSelected( Skill selectedSkill );
    static public event SkillSelected skillSelectedEvent;

    //Item usage
    private UseItemButton _SelectedShortcut;
    private Dictionary<KeyCode, UseItemButton> _ItemShortcuts = new Dictionary<KeyCode, UseItemButton>();


    //Cursors
    [SerializeField] private Texture2D _StandardCursor;
    [SerializeField] private Texture2D _InvalidTargetCursor;
    [SerializeField] private Texture2D _ValidTargetCursor;
    [SerializeField] private Texture2D _WalkCursor;
    [SerializeField] private Texture2D _AttackCursor;

    private void Awake()
    {
        if (Ref == null)
            Ref = this;
        else
            Destroy(this);
    }

    void Update ()
    {
        //Ignore clicks if the UI is being clicked
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Cursor.SetCursor(_StandardCursor, new Vector2(0, 0), CursorMode.Auto);
            return;
        }

        // PAUSE Ignore all input if the game is paused
        if (GameManager.pause) return;
        //--------------------------------------

        if ( _ActionSelected == ActionInput.SKILL || _ActionSelected == ActionInput.ITEM )
        {
            #region TARGET A SELECTED ACTION
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

            if (hit.collider != null)
            {
                TargetType targetType = GetTargetForSelectedAction();

                if ((targetType == TargetType.ENEMY && hit.collider.tag == "Enemy") ||
                     (targetType == TargetType.ALLY && hit.collider.tag == "Player"))
                {
                    Cursor.SetCursor(_ValidTargetCursor, new Vector2(32, 32), CursorMode.Auto);

                    if (Input.GetMouseButtonUp(0))
                    {
                        ActionTargetedToCreature(hit);
                    }
                }
                else
                {
                    Cursor.SetCursor(_InvalidTargetCursor, new Vector2(32, 32), CursorMode.Auto);

                    if (targetType == TargetType.GROUND)
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            ActionTargetedToGround(hit);
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            //Ivanlid target
                            print("Invalid target");

                            ResetActionSelected();
                        }
                    }
                }
            }
            #endregion
        }
        else
        {
            #region MOUSE_INPUT
            RaycastHit hit;
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

            if (hit.collider != null)
            {
                switch (hit.collider.tag)
                {
                    case "Terrain":
                        Cursor.SetCursor(_WalkCursor, new Vector2(0, 0), CursorMode.Auto);
                        if (Input.GetMouseButtonUp(0))
                            GiveMoveCommand(hit.point);
                        break;

                    case "Enemy":
                        Cursor.SetCursor(_AttackCursor, new Vector2(0, 0), CursorMode.Auto);
                        if (Input.GetMouseButtonUp(0))
                            SetTarget(hit.collider.gameObject);
                        break;

                    case "Player":
                        Cursor.SetCursor(_StandardCursor, new Vector2(0, 0), CursorMode.Auto);
                        if (Input.GetMouseButtonUp(0))
                            GameManager.SelectPlayer(hit.collider.gameObject);
                        break;

                    case "Interactable":
                        Cursor.SetCursor(_StandardCursor, new Vector2(0, 0), CursorMode.Auto);
                        if (Input.GetMouseButtonUp(0))
                            GiveInteractCommand(hit.collider.gameObject);
                        break;

                    default:
                        Cursor.SetCursor(_StandardCursor, new Vector2(0, 0), CursorMode.Auto);
                        break;
                }
            }
            #endregion
        }

#region SELECT_AND_SKILL_USE_BY_KEY
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameManager.SelectPlayer(GameManager.playerCreatures[0]);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (GameManager.playerCreatures.Count > 1)
                GameManager.SelectPlayer(GameManager.playerCreatures[1]);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (GameManager.playerCreatures.Count > 2)
                GameManager.SelectPlayer(GameManager.playerCreatures[2]);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (GameManager.playerCreatures.Count > 3)
                GameManager.SelectPlayer(GameManager.playerCreatures[3]);
        }
        //Use skills by key
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill( 0,0 );
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (GameManager.playerCreatures.Count > 1)
                UseSkill(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (GameManager.playerCreatures.Count > 2)
                UseSkill(2, 0);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (GameManager.playerCreatures.Count > 3)
                UseSkill(3, 0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameManager.playerCreatures.Count > 1)
                UseSkill(1, 1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (GameManager.playerCreatures.Count > 2)
                UseSkill(2, 1);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (GameManager.playerCreatures.Count > 3)
                UseSkill(3, 1);
        }


        else
        {
            foreach ( KeyValuePair<KeyCode, UseItemButton> pair in _ItemShortcuts )
            {
                if ( Input.GetKeyDown(pair.Key) )
                {
                    UseItem(pair.Value);
                    break;
                }
            }
        }
        #endregion
    }

    private TargetType GetTargetForSelectedAction()
    {
        TargetType targetType;
        if (_ActionSelected == ActionInput.SKILL)
            targetType = GameManager.playerCreatures[_CreatureIndex].GetComponent<CreatureSkillManager>().GetCooldownSkills()[_SkillIndex]._CurrentSkillInfo.targetType;
        else
            targetType = _SelectedShortcut.GetTargetType();
        return targetType;
    }

    private void ActionTargetedToGround(RaycastHit hit)
    {
        if (_ActionSelected == ActionInput.SKILL)
        {
            if (hit.collider != null)
                GameManager.playerCreatures[_CreatureIndex].GetComponent<CreatureIABasic>().AttackCommand(hit.point, _SkillIndex);
        }
        else if (_ActionSelected == ActionInput.ITEM)
        {
            if (hit.collider != null)
                _SelectedShortcut.GetItem().UseItem(hit.point);
        }

        ResetActionSelected();
    }

    private void ActionTargetedToCreature(RaycastHit hit)
    {
        if (_ActionSelected == ActionInput.SKILL)
        {
            if (hit.collider != null)
                GameManager.playerCreatures[_CreatureIndex].GetComponent<CreatureIABasic>().AttackCommand(hit.collider.gameObject, _SkillIndex);
        }
        else if (_ActionSelected == ActionInput.ITEM)
        {
            if (hit.collider != null)
                _SelectedShortcut.GetItem().UseItem(hit.collider.gameObject);
        }

        ResetActionSelected();
    }

    private void ResetActionSelected()
    {
        Cursor.SetCursor(_StandardCursor, new Vector2(0, 0), CursorMode.Auto);
        _ActionSelected = ActionInput.NONE;
        skillSelectedEvent?.Invoke(null);
    }

    void GiveMoveCommand( Vector3 destination )
    {
        if (_ControlMode == ControlMode.FOLLOW)
        {
            if (GameManager.selectedCreature != null)
            {
                GameManager.selectedCreature.GetComponent<CreatureIABasic>().MoveCommand(destination);
            }
        }
        else
        {
            //Calculating the normal
            Vector3 startPosition = GameManager.GetAveragePlayerPosition();
            Vector3 moveDirection = (destination - startPosition).normalized;
            Vector3 normal = Vector3.Cross(moveDirection, Vector3.up);

            GameManager.playerCreatures[0].GetComponent<CreatureIABasic>().MoveCommand(destination + (normal * 1.5f));
            GameManager.playerCreatures[1].GetComponent<CreatureIABasic>().MoveCommand(destination - (normal * 1.5f));
            if (GameManager.playerCreatures.Count > 2)
            {
                GameManager.playerCreatures[2].GetComponent<CreatureIABasic>().MoveCommand(destination + normal * 1.5f - moveDirection * 3f);
                if (GameManager.playerCreatures.Count > 3)
                {
                    GameManager.playerCreatures[3].GetComponent<CreatureIABasic>().MoveCommand(destination - normal * 1.5f - moveDirection * 3f);
                }
            }
        }
    }

    void GiveInteractCommand( GameObject interactable )
    {
        if (GameManager.selectedCreature != null)
        {
            GameManager.selectedCreature.GetComponent<CreatureIABasic>().InteractCommand(interactable);
        }
    }

    void SetTarget( GameObject target )
    {
        if (_ControlMode == ControlMode.FOLLOW)
        {
            if (GameManager.selectedCreature != null)
            {
                GameManager.selectedCreature.GetComponent<CreatureIABasic>().AttackCommand(target); ;
            }
        }
        else
        {
            foreach (GameObject go in GameManager.playerCreatures)
            {
                go.GetComponent<CreatureIABasic>().AttackCommand(target);
            }
        }
    }

    void UseSkill( int creatureIndex, int skillIndex )
    {
        CreatureIABasic creatureIABasic = GameManager.playerCreatures[creatureIndex].GetComponent<CreatureIABasic>();
        CreatureSkillManager skillManager = GameManager.playerCreatures[creatureIndex].GetComponent<CreatureSkillManager>();

        if (skillManager.GetCooldownSkills().Count > skillIndex && skillManager.GetCooldownSkills()[skillIndex] != null)
        {
            if (skillManager.GetCooldownSkills()[skillIndex]._CurrentSkillInfo.targetType == TargetType.SELF)
                creatureIABasic.UseSkill(skillIndex);
            else
            {
                _CreatureIndex = creatureIndex;
                _SkillIndex = skillIndex;
                _ActionSelected = ActionInput.SKILL;
                skillSelectedEvent?.Invoke(skillManager.GetCooldownSkills()[skillIndex]);
            }
        }
    }

    public void UseSkill( Skill skill )
    {
        int creatureIndex = GameManager.playerCreatures.IndexOf(skill.gameObject);
        int skillIndex = skill.GetComponent<CreatureSkillManager>().GetCooldownSkills().IndexOf(skill);

        UseSkill(creatureIndex, skillIndex);
    }

    public void UseItem( UseItemButton shortCutToUse )
    {
        _ActionSelected = ActionInput.ITEM;
        skillSelectedEvent?.Invoke(null);

        if (shortCutToUse.GetTargetType() == TargetType.SELF || shortCutToUse.GetTargetType() == TargetType.NONE)
            shortCutToUse.GetItem().UseItem();
        else
            _SelectedShortcut = shortCutToUse;
    }

    public void SetControlMode (bool controlModeFollow)
    {
        if (controlModeFollow)
            _ControlMode = ControlMode.FOLLOW;
        else
            _ControlMode = ControlMode.MIMIC;
    }

    public void SetItemShortcut(KeyCode newKey, UseItemButton itemButton)
    {
        if (_ItemShortcuts.ContainsKey(newKey) == false)
            _ItemShortcuts.Add(newKey, itemButton);
        else
            Debug.LogError("Keycode " + newKey.ToString() + " is already being used");
    }

    public void RemoveItemShortcut(UseItemButton itemButton)
    {
        // Just in case the dictionary is messed up and contains several itembuttons associated to several keys
        List<KeyCode> foundKeyCodes = new List<KeyCode>();

        foreach (KeyValuePair<KeyCode, UseItemButton> pair in _ItemShortcuts)
        {
            if ( pair.Value == itemButton )
                foundKeyCodes.Add( pair.Key );
        }

        foreach (KeyCode key in foundKeyCodes )
            _ItemShortcuts.Remove(key);
    }
}
