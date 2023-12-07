using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DisplayInfo
{
    GENERAL_ESSENCE,
    END
}

public class GUIInfoDisplay : MonoBehaviour
{
    public DisplayInfo _MyDisplayInfo;

    public Text _MyText;

    private void Awake()
    {
        _MyText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        switch (_MyDisplayInfo)
        {
            case DisplayInfo.GENERAL_ESSENCE:
                InventoryManager.Ref.EssenceAmountChangedEvent -= EssenceChanged;
                InventoryManager.Ref.EssenceAmountChangedEvent += EssenceChanged;
                break;
        }
        EssenceChanged(InventoryManager.Ref.GetEssence());
    }


    public void EssenceChanged ( int amount )
    {
        _MyText.text = amount.ToString();
    }

    public void OnDestroy()
    {
        if ( InventoryManager.Ref != null )
            InventoryManager.Ref.EssenceAmountChangedEvent -= EssenceChanged;
    }
}
