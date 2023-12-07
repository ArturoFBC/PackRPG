using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadGameMenu : MonoBehaviour
{
    public GameObject _ButtonPrefab;

    public Transform _ButtonParent;

    private void Start()
    {
        int[] ints = { 0, 1, 2 };

        for (uint i = 0; i < SaveLoadManager._SaveSlotAmount; i++)
        {
            SaveLoadManager.selectedSlot = i;
            SaveData saveData = SaveLoadManager.GetSave();

            GameObject newButton = Instantiate(_ButtonPrefab, _ButtonParent);

            ChangeSceneButton changeSceneButton = newButton.AddComponent<ChangeSceneButton>();
            Button newButtonBehaviour = newButton.GetComponentInChildren<Button>();
            int j = ints[i];
            newButtonBehaviour.onClick.AddListener(delegate { SelectSaveSlot(j); });

            if (saveData != null)
            {
                newButtonBehaviour.onClick.AddListener(delegate { changeSceneButton.GoToLair(); });
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0} - ", i) + "Continue";
            }
            else
            {
                newButtonBehaviour.onClick.AddListener(delegate { changeSceneButton.GoToStarterSelection(); });
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0} - ", i) + "New game";
            }
        }
    }

    public void SelectSaveSlot( int index )
    {
        SaveLoadManager.selectedSlot = (uint)index;
        DataManager.LoadData();
    }
}
