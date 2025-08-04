using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables.Dialogue
{
    [Serializable]
    public class Speaker
    {
        public string speakerName;
        public Sprite speakerAvatar;
    }

    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private List<Speaker> mySpeakers;
        [SerializeField] private Dialogue myDialogue;

        [SerializeField] private string displayName;

        public void SetDialogue(Dialogue newDialogue)
        {
            myDialogue = newDialogue;
        }

        public void Interact(Transform whoActivatedMe)
        {
            if (mySpeakers.Count > 1)
                transform.LookAt(whoActivatedMe);

            DialogManager.Ref.DisplayDialogue(myDialogue, mySpeakers);
        }

        internal Dialogue GetDialogue()
        {
            return myDialogue;
        }

        public string GetDisplayName()
        {
            if (string.IsNullOrEmpty(displayName))
            {
                if (mySpeakers != null && mySpeakers.Count > 0)
                    return mySpeakers[0].speakerName;
                else
                    return gameObject.name;
            }

            return displayName;

        }
    }
}
