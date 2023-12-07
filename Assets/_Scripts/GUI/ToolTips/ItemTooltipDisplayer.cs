using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTooltipDisplayer : TooltipDisplayer
{
    protected override void SendSetTooltip()
    {
        if (_ObjectToDisplay is Item)
            _InstantiatedTooltip.SendMessage("SetItem", (Item)_ObjectToDisplay, SendMessageOptions.DontRequireReceiver);
    }
}
