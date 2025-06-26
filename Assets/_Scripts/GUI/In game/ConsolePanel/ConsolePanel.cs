using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.InGame.Console
{
    public class ConsolePanel : Singleton<ConsolePanel>
    {
        private const int MAX_MESSAGES = 10;

        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private CanvasGroup myCanvasGroup;

        private Queue<ConsoleMessage> messages = new Queue<ConsoleMessage>();

        [SerializeField] private float displayTime = 10f;
        private float displayCounter = 0f;
        [SerializeField] private float transitionSpeed = 5f;
        [SerializeField] private Toggle myToggle;

        private void Update()
        {
            if (displayCounter > 0f)
            {
                displayCounter -= Time.deltaTime;

                if (displayCounter <= 0f)
                {
                    myToggle.isOn = false;
                }
            }
        }

        public void DisplayMessage(string message)
        {
            if (messages.Count > MAX_MESSAGES)
            {
                ConsoleMessage oldestMessage = messages.Dequeue();
                Destroy(oldestMessage);
            }

            ConsoleMessage newConsoleMessage = Instantiate(messagePrefab, messageContainer).GetComponent<ConsoleMessage>();
            newConsoleMessage.SetMessage(message);
            messages.Enqueue(newConsoleMessage);

            myToggle.isOn = true;
        }

        public void OnToggleSwitch(bool toggleState)
        {
            if (toggleState)
            {
                Show();
            }
            else 
            {
                Hide();
            }
        }

        public void Show()
        {
            displayCounter = displayTime;

            StartCoroutine("RaiseAlpha");
        }

        private IEnumerator RaiseAlpha()
        {
            while (myCanvasGroup.alpha < 1)
            {
                myCanvasGroup.alpha += Time.deltaTime * transitionSpeed;

                if (myCanvasGroup.alpha > 1)
                    myCanvasGroup.alpha = 1;

                yield return new WaitForEndOfFrame();
            }
        }

        public void Hide()
        {
            displayCounter = 0;

            StartCoroutine("LowerAlpha");
        }

        private IEnumerator LowerAlpha()
        {
            while (myCanvasGroup.alpha > 0)
            {
                myCanvasGroup.alpha -= Time.deltaTime * transitionSpeed;

                if (myCanvasGroup.alpha < 0)
                    myCanvasGroup.alpha = 0;

                yield return new WaitForEndOfFrame();
            }
        }
    }


}