using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NewDialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<Speech> saveSpeeches = new List<Speech>();
        [SerializeField] private List<int> saveSpeechIDs = new List<int>();

        [SerializeField] private Dictionary<int, Speech> speeches;


#if UNITY_EDITOR
        private void Awake()
        {
            FillDictionary();
            AvoidZeroElementList();
        }

        private void OnValidate()
        {
            FillDictionary();
            AvoidZeroElementList();

            if (saveSpeeches.Count != speeches.Count)
                SaveSpeeches();
        }

        private void SaveSpeeches()
        {
            saveSpeeches.Clear();
            saveSpeechIDs.Clear();
            foreach (KeyValuePair<int,Speech> pair in speeches)
            {
                saveSpeeches.Add(pair.Value);
                saveSpeechIDs.Add(pair.Key); 
            }
        }

        private void FillDictionary()
        {
            speeches = new Dictionary<int, Speech>();
            if (saveSpeeches != null && saveSpeeches.Count > 0)
            {
                for (int i = 0; i < saveSpeeches.Count; i++)
                    speeches.Add(i, saveSpeeches[i]);
            }
        }

        private void AvoidZeroElementList()
        {
            if (speeches == null)
                speeches = new Dictionary<int, Speech>();

            if (speeches.Count == 0)
                speeches.Add(0, new Speech());
        }

        public IEnumerable<Speech> GetSpeeches()
        {
            return speeches.Values;
        }

        public Speech GetStartingSpeech()
        {
            return speeches[0];
        }

        public IEnumerable<Speech> GetChildrenOfSpeech(Speech speech)
        {
            if (speech.childrenIDs != null)
            foreach (int speechID in speech.childrenIDs)
                yield return speeches[speechID];
        }

        public void CreateChildOfSpeech(Speech childSpeechRequested)
        {
            int newSpeechID = GetFreeID();

            childSpeechRequested.childrenIDs.Add(newSpeechID);
            speeches.Add(newSpeechID, new Speech());

            SaveSpeeches();
        }

        public int GetFreeID()
        {
            for (int i = 0; i < speeches.Count; i++)
            {
                if (speeches.ContainsKey(i) == false)
                    return i;
            }
            return speeches.Count;
        }
#endif
    }

}