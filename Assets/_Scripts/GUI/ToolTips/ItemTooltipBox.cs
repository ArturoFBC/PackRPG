using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipBox : MonoBehaviour
{
    public Text _Name, _Description;
    public Image _Icon;

    public void SetItem(Item item)
    {
        // COMMON
        print(item.GetName());
        _Name.text = item.GetName();
        _Description.text = item.GetDescription();
        _Icon.sprite = item.GetIcon();
    }
}
