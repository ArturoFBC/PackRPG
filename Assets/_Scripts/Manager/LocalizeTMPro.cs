using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizeTMPro : MonoBehaviour
{
    private TextMeshProUGUI myText;

    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        myText.text = LanguageManager.Localize(myText.text);
    }
}
