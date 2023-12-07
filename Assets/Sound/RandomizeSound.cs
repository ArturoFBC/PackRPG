using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeSound : MonoBehaviour
{
    public AudioClip[] sounds;

    void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null && sounds.Length > 0)
        {
            audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.enabled = true;
        }
    }
}
