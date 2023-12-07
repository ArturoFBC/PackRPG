using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverIndicator : MonoBehaviour
{
    [SerializeField] private Material _MyMaterial;

    private MouseOverManager myMouseOver;


    private void Start()
    {
        if (_MyMaterial == null)
            _MyMaterial = GetComponentInChildren<Renderer>().material;
    }

    public void MouseOverStart()
    {
        if (_MyMaterial != null)
            _MyMaterial.SetFloat("_Border", 2f);
    }

    public void MouseOverEnded()
    {
        if (_MyMaterial != null)
            _MyMaterial.SetFloat("_Border", 0f);
    }

    public void SetTier (EnemyTier tier)
    {
        if (_MyMaterial != null)
            _MyMaterial.SetColor("_RimColor", EnemyTierValues.color[(int)tier]);
    }
}
