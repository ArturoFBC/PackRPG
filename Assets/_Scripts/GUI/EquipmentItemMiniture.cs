using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipmentItemMiniture : ItemMiniature
{
    public delegate void EquipmentItemClicked(Item miniature);
    static public event EquipmentItemClicked EquipmentItemClickedEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        EquipmentItemClickedEvent?.Invoke(_MyItem);
    }
}
