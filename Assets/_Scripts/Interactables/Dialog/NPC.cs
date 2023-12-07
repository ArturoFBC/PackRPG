using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private Dialogue myDialogue;

        public void SetDialogue(Dialogue newDialogue)
        {
            myDialogue = newDialogue;
        }

        public void Interact(Transform whoActivatedMe)
        {
            this.transform.LookAt(whoActivatedMe);
            DialogManager.Ref.DisplayDialogue(myDialogue);
        }
    }
}
