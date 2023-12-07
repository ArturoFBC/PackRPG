using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeciesTooltipDisplayer : TooltipDisplayer
{
    protected override void SendSetTooltip()
    {
        if (_ObjectToDisplay is Species)
        {
            _InstantiatedTooltip.SendMessage("SetSpecies", (Species)_ObjectToDisplay);
        }
    }
}
