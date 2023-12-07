using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMarkDisplayer : MonoBehaviour {

    public GameObject selectedMarkPrefab;
    private GameObject selectedMarkGO;
    private SelectedMark selectedMark;

    private void Awake()
    {
        CreateSelectionMark();
    }

    private void CreateSelectionMark()
    {
        selectedMarkGO = Instantiate(selectedMarkPrefab, gameObject.transform, false);
    }
}
