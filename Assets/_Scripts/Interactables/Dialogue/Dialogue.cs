using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Interactables.Dialogue
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

        public void SaveSpeeches()
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
                    speeches.Add(saveSpeechIDs[i], saveSpeeches[i]);
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

        public IEnumerable<Speech> GetChildrenOfSpeech(Speech speech)
        {
            if (speech.childrenIDs != null)
            foreach (int speechID in speech.childrenIDs)
                yield return speeches[speechID];
        }

        public void CreateChildOfSpeech(Speech parentSpeech)
        {
            int newSpeechID = GetFreeID();

            parentSpeech.childrenIDs.Add(newSpeechID);
            speeches.Add(newSpeechID, new Speech());

            Vector2 childPosition = parentSpeech.editorPosition.position + new Vector2(Speech.NEW_SPEECH_OFFSET, 0f);
            speeches[newSpeechID].editorPosition.position = childPosition;
            speeches[newSpeechID].speakerID = parentSpeech.speakerID == 0 ? 1 : 0;

            SaveSpeeches();
        }

        public void DeleteSpeech(Speech speech)
        {
            int deletedID = ReverseDictionaryLookUp(speech);

            speeches.Remove(deletedID);
            foreach (KeyValuePair<int, Speech> pair in speeches)
            {
                if (pair.Value.childrenIDs.Contains(deletedID))
                    pair.Value.childrenIDs.Remove(deletedID);
            }

            SaveSpeeches();
        }

        public int ReverseDictionaryLookUp(Speech speech)
        {
            foreach (KeyValuePair<int, Speech> pair in speeches)
            {
                if (pair.Value == speech)
                    return pair.Key;
            }

            return 0;
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

        public void ReparentSpeech(Speech parentSpeech, Speech childSpeech)
        {
            // Do not link to itself
            if (parentSpeech == childSpeech)
                return;

            int childID = ReverseDictionaryLookUp(childSpeech);

            // Do not link if already linked
            if (parentSpeech != null && parentSpeech.childrenIDs.Contains(childID))
                return;

            // Remove all links
            foreach (KeyValuePair<int, Speech> pair in speeches)
            {
                if (pair.Value.childrenIDs.Contains(childID))
                    pair.Value.childrenIDs.Remove(childID);
            }

            // Relink
            if (parentSpeech != null)
                parentSpeech.childrenIDs.Add(childID);

            SaveSpeeches();
        }
#endif

        internal Speech GetSpeech(int speechIndex)
        {
            return speeches[speechIndex];
        }
    }

}