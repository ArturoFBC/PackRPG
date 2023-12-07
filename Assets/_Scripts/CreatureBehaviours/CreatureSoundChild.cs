using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSoundChild : MonoBehaviour
{
    //Since the animator component will be located in a child gameobject, it can not call functions in the Creature Sound script. This script forwards the messages to CreatureSound on the parent gameobject

    private CreatureSound _ParentCreatureSound;

    private void Awake()
    {
        if (_ParentCreatureSound == null)
            _ParentCreatureSound = GetComponentInParent<CreatureSound>();
    }

    public void PlayFootstep()
    {
        _ParentCreatureSound.PlayFootstep();
    }
}
