using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{

    public class DialogManager : Singleton<DialogManager>
    {
        [SerializeField] private Text speakerNameLabel;
        [SerializeField] private Text speechLabel;
        [SerializeField] private Image speakerAvatar;

        [SerializeField] private List<Button> answerButtons;
        [SerializeField] private Button continueButton;

        private Dialogue currentDialogue;
        private Speech currentSpeech;
        private List<Speaker> currentSpeakers;

        private List<string> remainingSpeech = new List<string>();

        public delegate void SpeechEnded();
        public event SpeechEnded SpeechEndedEvent;
        public delegate void DialogEnded();
        public event DialogEnded DialogEndedEvent;

        protected override void InheritedAwake()
        {
            gameObject.SetActive(false);
        }

        public void DisplayDialogue(Dialogue dialogue, List<Speaker> speakers)
        {
            currentDialogue = dialogue;
            currentSpeakers = speakers;

            gameObject.SetActive(true);
            StartSpeech(dialogue.GetSpeech(0));
        }

        private void StartSpeech(Speech turn)
        {
            currentSpeech = turn;

            ConfigureUIForSpeech();

            if (turn.speakerID >= currentSpeakers.Count)
                Debug.LogError("Dialogue error in -" + currentDialogue.name + "- dialogue, speakerIndex in speech " + currentSpeech.text + " does not exist");

            Speaker speaker = currentSpeakers[turn.speakerID];
            DisplaySpeaker(speaker.speakerAvatar, speaker.speakerName);

            List<string> words = new List<string>(turn.text.Split(' '));
            DisplayText(words);
        }

        private void DisplayText(List<string> words)
        {
            speechLabel.text = "";

            bool complete = true;
            for (int i = 0; i < words.Count; i++)
            {
                if (i > 0) speechLabel.text += ' ';
                string tempText = speechLabel.text;
                speechLabel.text += words[i];
                if (speechLabel.IsOverflowingVerticaly())
                {
                    speechLabel.text = tempText;
                    remainingSpeech = new List<string>(words.Skip(i));
                    complete = false;
                    break;
                }
            }
            if (complete)
                remainingSpeech.Clear();
        }

        private void DisplaySpeaker(Sprite avatar, string name)
        {
            speakerNameLabel.text = name;
            speakerAvatar.sprite = avatar;
        }

        public void ContinueSpeech()
        {
            if (remainingSpeech.Count <= 0)
            {
                SpeechEndedEvent?.Invoke();
                ContinueDialogue();
            }
            else
            { 
                DisplayText(remainingSpeech);
            }
        }

        private void ContinueDialogue()
        {

            if (currentSpeech.childrenIDs.Count > 1)
            {
                DisplayAnswers();
            }
            else if (currentSpeech.childrenIDs.Count == 1)
            {
                Debug.Log("Display next speech");
                currentSpeech = currentDialogue.GetSpeech(currentSpeech.childrenIDs[0]);
                StartSpeech(currentSpeech);
            }
            else
            {
                DialogEndedEvent?.Invoke();
                gameObject.SetActive(false);
            }
        }

        private void DisplayAnswers()
        {
            int speakerIndex = 0;

            Debug.Log(currentSpeech.text);
            for (int i = 0; i< currentSpeech.childrenIDs.Count; i++)
            {
                Debug.Log("Display answers " + i);
                Speech answer = currentDialogue.GetSpeech(currentSpeech.childrenIDs[i]);
                answerButtons[i].GetComponentInChildren<Text>().text = answer.text;
                answerButtons[i].gameObject.SetActive(true);
                speakerIndex = answer.speakerID;
            }

            continueButton.gameObject.SetActive(false);
            speechLabel.transform.parent.gameObject.SetActive(false);

            Speaker speaker = currentSpeakers[speakerIndex];
            DisplaySpeaker(speaker.speakerAvatar, speaker.speakerName);
        }

        public void AnswerSelected(int answerIndex)
        {
            Speech answerSpeech = currentDialogue.GetSpeech(currentSpeech.childrenIDs[answerIndex]);

            StartSpeech(answerSpeech);
        }

        private void ConfigureUIForSpeech()
        {
            for (int i = 0; i < answerButtons.Count; i++)
                answerButtons[i].gameObject.SetActive(false);

            continueButton.gameObject.SetActive(true);
            speechLabel.transform.parent.gameObject.SetActive(true);
        }
    }
}
