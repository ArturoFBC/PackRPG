using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour
{
    [SerializeField]
    private Text _DisplayText;

    public void UpdateLevel(int level)
    {
        _DisplayText.text = level.ToString();
    }
}