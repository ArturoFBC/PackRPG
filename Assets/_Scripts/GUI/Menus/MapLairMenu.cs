using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameProgress;
using TMPro;

public class MapLairMenu : MonoBehaviour
{
    public Transform _ButtonsParent;
    public GameObject _ButtonPrefab;

    private void OnEnable()
    {
        // Clear menu contents
        foreach ( Transform child in _ButtonsParent)
            Destroy(child.gameObject);

        // Add and configure area buttons
        foreach ( KeyValuePair<Area,bool> areaPair in GameProgressManager.GetAllAreasUnlockStatus() )
        {
            GameObject newButtonGameObject = Instantiate(_ButtonPrefab, _ButtonsParent);
            Button newButtonBehaviour = newButtonGameObject.GetComponentInChildren<Button>();
            ChangeSceneButton newChangeSceneBehaviour = newButtonBehaviour.gameObject.AddComponent<ChangeSceneButton>();

            newButtonBehaviour.onClick.AddListener( delegate {    newChangeSceneBehaviour.GoToArea(areaPair.Key);    } );
            newButtonBehaviour.interactable = areaPair.Value;
            newButtonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = areaPair.Key.name + " - Level " + areaPair.Key.areaLevel + " area";
        }
    }
}
