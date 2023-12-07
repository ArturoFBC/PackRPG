using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnInteract : MonoBehaviour
{
    [SerializeField] private Animation myAnimation;
    
    private void Awake()
    {
        if (myAnimation == null)
            myAnimation = GetComponent<Animation>();
    }

    public void Interact()
    {
        myAnimation.Play();
    }
}
