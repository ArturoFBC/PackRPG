using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NewDialogue
{
    [System.Serializable]
    public class Speech
    {
        public const float NEW_SPEECH_OFFSET = 300f;

        public string text;
        public List<int> childrenIDs = new List<int>();
        public int speakerID;

        //Inspector
        public Rect editorPosition = new Rect(0,0,250,90);
    }
}