using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Interface.InGame.Console
{
    public class ConsoleMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI myText;

        private void Awake()
        {
            myText = GetComponent<TextMeshProUGUI>();
        }

        public void SetMessage(string message)
        {
            myText.text = message;
        }
    }
}