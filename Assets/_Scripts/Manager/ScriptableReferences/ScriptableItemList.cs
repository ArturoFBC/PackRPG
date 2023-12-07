using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemList")]
public class ScriptableItemList : ScriptableObject
{
    public List<BaseItem> items;
}
