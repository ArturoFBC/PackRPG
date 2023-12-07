using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play2DSound : MonoBehaviour
{
    public SoundID _Sound;
    public bool _PlayOnStart;

    void Start()
    {
        if ( _PlayOnStart ) GlobalSounds._Ref.Play(_Sound);
    }

    public void Play()
    {
        GlobalSounds._Ref.Play(_Sound);
    }
}
