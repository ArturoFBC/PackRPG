using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecimenTooltipDisplayer : TooltipDisplayer
{
    protected override void SendSetTooltip()
    {
        if (_ObjectToDisplay is Specimen)
        {
            _InstantiatedTooltip.SendMessage("DisplaySpecimen", _ObjectToDisplay);
        }
    }
}
