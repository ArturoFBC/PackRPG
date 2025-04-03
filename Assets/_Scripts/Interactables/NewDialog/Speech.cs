using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NewDialogue
{
    [System.Serializable]
    public class Speech
    {
        public string text;
        public List<int> childrenIDs = new List<int>();

        //Inspector
        public Rect editorPosition = new Rect(0,0,200,70);
    }
}