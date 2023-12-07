using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUnpauseButton : MonoBehaviour
{
    public void Pause( bool pause )
    {
        if (pause)
            GameManager.Pause();
        else
            GameManager.Unpause();
    }
}
