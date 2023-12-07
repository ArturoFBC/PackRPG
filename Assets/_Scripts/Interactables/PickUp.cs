using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PickUp : MonoBehaviour, IInteractable
{
    protected const float AUTO_PICKUP_RADIUS = 3f;

    [SerializeField] private float _DestructionDuration = 0.3f;

    public abstract void Interact(Transform whoActivatedMe);

    protected void DisplayVisualEffects(Transform whoActivatedMe)
    {
        TransformChange tc = gameObject.GetComponent<TransformChange>();
        TransformChangeInfo tci = new TransformChangeInfo();

        tci.transform.position = whoActivatedMe.position;
        tci.transform.scale = Vector3.zero;
        tci.time = _DestructionDuration;
        tci.world = true;

        tc.NonStaticStartTransformChange(tci);

        DestroyInXseconds dix = gameObject.AddComponent<DestroyInXseconds>();
        dix.timeToBeDestroyed = _DestructionDuration;
    }

    protected void DisplayAudioEffect()
    {
        AudioSource myAudio = GetComponent<AudioSource>();
        if (myAudio != null) myAudio.Play();
    }

    protected void SetAutoPickUp( float radius )
    {
        SphereCollider myTrigger = gameObject.AddComponent<SphereCollider>();
        myTrigger.isTrigger = true;
        myTrigger.center = default;
        myTrigger.radius = radius;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponentInChildren<CreatureStats>() != null)
        {
            Interact(other.transform);
        }
    }
}
