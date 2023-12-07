using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundID
{
    LevelUp,
    ButtonPress,
    CloseMenu,
    OpenMenu,
    END
}

[System.Serializable]
public class Sound
{
    public AudioClip clip;

    [Range (0,1)]
    public float volume;

    [Range(0.1f, 3)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}

public class GlobalSounds : MonoBehaviour
{
    public static GlobalSounds _Ref;

    public Sound[] sounds;

    private void Awake()
    {
        if (_Ref == null)
            _Ref = this;
        else
        {
            Destroy(this);
            return;
        }

        foreach ( Sound s in sounds )
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(SoundID soundID)
    {
        if ( sounds != null && 
             sounds.Length >= (int)soundID &&
             sounds[(int)soundID].source != null)
        {
            sounds[(int)soundID].source.Play();
        }
    }
}

