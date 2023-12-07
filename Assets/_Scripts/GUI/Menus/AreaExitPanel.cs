using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AreaExitPanel : MonoBehaviour
{
    public GameObject _NextAreaButton;

    public void ShowPanel( Area nextArea )
    {
        Button newButtonBehaviour = _NextAreaButton.GetComponentInChildren<Button>();
       
        ChangeSceneButton changeSceneBehaviour = newButtonBehaviour.gameObject.GetComponent<ChangeSceneButton>();
        if ( changeSceneBehaviour == null )
            changeSceneBehaviour = newButtonBehaviour.gameObject.AddComponent<ChangeSceneButton>();

        newButtonBehaviour.onClick.AddListener(delegate { changeSceneBehaviour.GoToArea(nextArea); });

        _NextAreaButton.GetComponentInChildren<TextMeshProUGUI>().text = nextArea.name + " - Level " + nextArea.areaLevel + " area";

        gameObject.SetActive(true);
    }
}
