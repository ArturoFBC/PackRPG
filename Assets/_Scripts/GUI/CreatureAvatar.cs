using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureAvatar : MonoBehaviour
{
    enum MarkStatus
    {
        SELECTED,
        NOT_SELECTED,
        KO
    }

    [SerializeField] private Image avatarSelectionIndicator;

    private GameObject myCreature;

    [SerializeField] private Color selectedColor   = Color.green;
    [SerializeField] private Color unselectedColor = Color.black;
    [SerializeField] private Color koColor         = Color.red;

    private bool _Selected;
    private bool _KO;

    private void Awake()
    {
        if ( avatarSelectionIndicator == null )
            avatarSelectionIndicator = GetComponent<Image>();
    }

    public void OnEnable()
    {
        GameManager.creatureSelectedEvent -= OnCreatureSelected;
        GameManager.creatureSelectedEvent += OnCreatureSelected;
    }

    public void Set(GameObject selectedCreature)
    {
        UnlinkKOEvents(myCreature);
        myCreature = selectedCreature;
        LinkKOEvents(myCreature);
    }
    
    public void OnCreatureSelected (GameObject selectedCreature)
    {
        _Selected = (myCreature == selectedCreature);
        SetMark(_Selected, _KO);
    }

    public void OnKO()
    {
        _KO = true;
        SetMark(_Selected, _KO);
    }

    public void OnArise()
    {
        _KO = false;
        SetMark(_Selected, _KO);
    }

    private void SetMark(bool selected, bool ko)
    {
        if (ko)
            avatarSelectionIndicator.color = koColor;
        else if (selected)
            avatarSelectionIndicator.color = selectedColor;
        else
            avatarSelectionIndicator.color = unselectedColor;
    }

    private void UnlinkKOEvents(GameObject targetCreature)
    {
        if (targetCreature != null)
        {
            CreatureHitPoints myHitPoints = targetCreature.GetComponent<CreatureHitPoints>();
            if (myHitPoints != null)
            {
                myHitPoints.KnockOutEvent -= OnKO;
                myHitPoints.AriseEvent -= OnArise;
            }
        }
    }

    private void LinkKOEvents(GameObject targetCreature)
    {
        if (targetCreature != null)
        {
            CreatureHitPoints myHitPoints = targetCreature.GetComponent<CreatureHitPoints>();
            if (myHitPoints != null)
            {
                myHitPoints.KnockOutEvent += OnKO;
                myHitPoints.AriseEvent += OnArise;
            }
        }
    }

    public void SelectMyCreature()
    {
        GameManager.SelectPlayer(myCreature);
    }

    public void OnDisable()
    {
        GameManager.creatureSelectedEvent -= OnCreatureSelected;
        UnlinkKOEvents(myCreature);
    }
}
