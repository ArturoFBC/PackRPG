using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSound : MonoBehaviour
{
    private AudioSource _MyAudioSource;

    public AudioClip[] _FootSteps;

    private void Awake()
    {
        if (_MyAudioSource == null)
            _MyAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Add an script to the gameobject where the animation is located to forward the sound messages to this script;
        Animation animation = GetComponentInChildren<Animation>();
        if (animation != null && animation.GetComponent<CreatureSoundChild>() == null)
        {
            animation.gameObject.AddComponent<CreatureSoundChild>();
        }

        // Enable animator events
        Animator animator = GetComponentInChildren<Animator>();
        if (animator != null)
            animator.fireEvents = true;

        // Get the footstep sounds
        CreatureStats myCreatureStats = GetComponent<CreatureStats>();

        Species mySpecies = null;
        mySpecies = (myCreatureStats).GetSpecimen().species;

        if (mySpecies.stepSounds != null && mySpecies.stepSounds.Length > 0)
            _FootSteps = mySpecies.stepSounds;
    }

    public void PlayFootstep()
    {
        if ( _MyAudioSource != null )
            _MyAudioSource.PlayOneShot( _FootSteps[Random.Range(0, _FootSteps.Length)] );
    }

    public void PlayOneShot( AudioClip audioClip)
    {
        print(audioClip.name);
        _MyAudioSource.PlayOneShot(audioClip);
    }
}
