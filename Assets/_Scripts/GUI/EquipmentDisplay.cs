using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDisplay : MonoBehaviour
{
    [SerializeField] private List<EquipmentSlotDisplay> slotDisplays;
    [SerializeField] private GameObject slotDisplayPrefab;

    [SerializeField] private Transform slotDisplayContainer;

    public void Display(List<EquipmentSlot> slots)
    {
        Clear();
        foreach(EquipmentSlot slot in slots)
        {
            EquipmentSlotDisplay newSlotDisplay = Instantiate(slotDisplayPrefab, slotDisplayContainer).GetComponent<EquipmentSlotDisplay>();
            newSlotDisplay.Display(slot);
            slotDisplays.Add(newSlotDisplay);
        }
    }

    public void Clear()
    {
        foreach (EquipmentSlotDisplay slotDisplay in slotDisplays)
            Destroy(slotDisplay.gameObject);

        slotDisplays.Clear();
    }
}
