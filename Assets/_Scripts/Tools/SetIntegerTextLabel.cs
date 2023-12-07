using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SetIntegerTextLabel : MonoBehaviour
{
    private Text myText;

    public void SetText( float value )
    {
        if (myText == null)
            myText = GetComponent<Text>();

        myText.text = value.ToString();
    }

}
