using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to attach to avatar dummies, if it doesnt detect a "creature stats" component, it implies that it is a dummy and should not use sound events
public class DummyBehaviour : MonoBehaviour
{
    void Awake()
    {
        if (GetComponent<CreatureStats>() == null)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
                animator.fireEvents = false;
        }
        Destroy(this);
    }
}
