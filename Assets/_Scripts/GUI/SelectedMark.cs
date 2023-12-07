using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[RequireComponent (typeof(DecalProjector))]
public class SelectedMark : MonoBehaviour {

    enum MarkStatus
    {
        SELECTED,
        NOT_SELECTED,
        KO
    }

    public Material[] _MarkMaterials;
    private DecalProjector _MyProjector;

    [SerializeField] private GameObject myCreature;

    private bool _Selected;
    private bool _KO;

    private void Awake()
    {
        if (_MyProjector == null)
            _MyProjector = GetComponent<DecalProjector>();

        CreatureHitPoints myHitPoints = GetComponentInParent<CreatureHitPoints>();
        if (myHitPoints != null)
        {
            myHitPoints.KnockOutEvent += OnKO;
            myHitPoints.AriseEvent += OnArise;

            myCreature = myHitPoints.gameObject;

            GameManager.creatureSelectedEvent += SetSelected;
        }
    }

    public void SetSelected( GameObject selectedCreature )
    {
        bool selected = (selectedCreature == myCreature);
        
        _Selected = selected;
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

    private void SetMark( bool selected, bool ko )
    {
        if ( ko )
            _MyProjector.material = _MarkMaterials[(int)MarkStatus.KO];
        else if (selected)
            _MyProjector.material = _MarkMaterials[(int)MarkStatus.SELECTED];
        else
            _MyProjector.material = _MarkMaterials[(int)MarkStatus.NOT_SELECTED];
    }

    public void OnDestroy()
    {
        GameManager.creatureSelectedEvent -= SetSelected;
    }
}
