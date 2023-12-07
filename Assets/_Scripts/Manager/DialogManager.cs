using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{

    public class DialogManager : Singleton<DialogManager>
    {
        [SerializeField] private Text speakerNameLabel;
        [SerializeField] private Text speechLabel;
        [SerializeField] private Image speakerAvatar;

        private Dialogue currentDialogue;
        private int currentDialogueIndex;

        private List<string> remainingSpeech = new List<string>();

        public delegate void SpeechEnded();
        public event SpeechEnded SpeechEndedEvent;
        public delegate void DialogEnded();
        public event DialogEnded DialogEndedEvent;

        protected override void InheritedAwake()
        {
            gameObject.SetActive(false);
        }

        public void DisplayDialogue(Dialogue dialogue)
        {
            currentDialogue = dialogue;
            currentDialogueIndex = 0;

            gameObject.SetActive(true);
            StartSpeech(currentDialogue.speeches[currentDialogueIndex]);
        }

        private void StartSpeech(DialogueTurn turn)
        {
            if (turn.speakerIndex >= currentDialogue.speakers.Count)
                Debug.LogError("Dialogue error in -" + currentDialogue.name + "- dialogue, speakerIndex in turn " + currentDialogueIndex + " does not exist");

            Speaker speaker = currentDialogue.speakers[turn.speakerIndex];
            DisplaySpeaker(speaker.speakerAvatar, speaker.speakerName);

            List<string> words = new List<string>(turn.speech.Split(' '));
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
            if (remainingSpeech.Count > 0)
                DisplayText(remainingSpeech);
            else
            {
                SpeechEndedEvent?.Invoke();
                ContinueDialogue();
            }
        }

        private void ContinueDialogue()
        {
            currentDialogueIndex++;
            if (currentDialogueIndex < currentDialogue.speeches.Count)
                StartSpeech(currentDialogue.speeches[currentDialogueIndex]);
            else
            {
                DialogEndedEvent?.Invoke();
                gameObject.SetActive(false);
            }
        }
    }
}
