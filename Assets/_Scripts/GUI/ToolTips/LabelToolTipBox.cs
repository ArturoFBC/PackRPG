using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelToolTipBox : MonoBehaviour
{
    [SerializeField] Text textBox;

    public void Awake()
    {
        if (textBox == null)
            textBox = GetComponentInChildren<Text>();
    }

    public void SetText( string textToDisplay )
    {
        textBox.text = textToDisplay;
    }
}
