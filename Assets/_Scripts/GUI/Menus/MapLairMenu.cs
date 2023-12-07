using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        foreach ( AreaUnlock areaUnlock in GameProgress.Ref.GetAreaList() )
        {
            GameObject newButtonGameObject = Instantiate(_ButtonPrefab, _ButtonsParent);
            Button newButtonBehaviour = newButtonGameObject.GetComponentInChildren<Button>();
            ChangeSceneButton newChangeSceneBehaviour = newButtonBehaviour.gameObject.AddComponent<ChangeSceneButton>();

            newButtonBehaviour.onClick.AddListener( delegate {    newChangeSceneBehaviour.GoToArea(areaUnlock.area);    } );
            newButtonBehaviour.interactable = areaUnlock.unlocked;
            newButtonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = areaUnlock.area.name + " - Level " + areaUnlock.area.areaLevel + " area";
        }
    }
}
