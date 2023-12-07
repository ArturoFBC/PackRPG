using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(CreatureDisplay))]
public class SelectCreatureButton : MonoBehaviour
{
    CreatureDisplay _MyDisplayCreature;
    
    void Start()
    {
        if (_MyDisplayCreature == null)
            _MyDisplayCreature = GetComponent<CreatureDisplay>();
    }

    public void SelectThisCreature()
    {
        Specimen _MySpecimen = _MyDisplayCreature.GetSpecimen();

        GetComponentInParent<CreatureSelection>().CreatureSwitch(_MySpecimen);
    }
}
