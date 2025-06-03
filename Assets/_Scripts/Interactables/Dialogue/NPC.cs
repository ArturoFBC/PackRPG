using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
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

        public void SetDialogue(Dialogue newDialogue)
        {
            myDialogue = newDialogue;
        }

        public void Interact(Transform whoActivatedMe)
        {
            this.transform.LookAt(whoActivatedMe);
            DialogManager.Ref.DisplayDialogue(myDialogue, mySpeakers);
        }
    }
}
