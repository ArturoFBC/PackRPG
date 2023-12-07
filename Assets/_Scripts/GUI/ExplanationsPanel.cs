using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanationsPanel : MonoBehaviour
{
    static private bool _Shown = false;

    private void OnEnable()
    {
        if (_Shown)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        _Shown = true;
    }

}
