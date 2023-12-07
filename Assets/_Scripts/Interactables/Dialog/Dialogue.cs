using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Dialogue
{
    [Serializable]
    public struct DialogueTurn
    {
        public int speakerIndex;
        public string speech;
    }

    [Serializable]
    public class Speaker
    {
        public string speakerName;
        public Sprite speakerAvatar;
    }

    [CreateAssetMenu(menuName = "Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public List<Speaker> speakers;
        public List<DialogueTurn> speeches;
    }
}