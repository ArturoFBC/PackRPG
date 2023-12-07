using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(TransformChange))]
public class ArcIntoPosition : MonoBehaviour
{
    [SerializeField] private TransformChange myTransformChange;
    [SerializeField] private Vector3 middlePointOffset = new Vector3(0,1,0);
    [SerializeField] private float totalTime = 1f;

    [SerializeField] private Vector3 destination;

    private void Awake()
    {
        if (myTransformChange == null)
            myTransformChange = GetComponent<TransformChange>();
    }

    public void Set( Vector3 setDestination )
    {
        destination = setDestination;
        this.enabled = true;
    }

    private void OnEnable()
    {
        if (destination == Vector3.zero)
        {
            enabled = false;
            return;
        }

        Vector3 middlePoint = ((transform.position + destination) / 2f) + middlePointOffset;
        NoPointerTransform nonPointerMiddlePoint = new NoPointerTransform()
        {
            position = middlePoint,
            rotation = transform.rotation,
            scale    = transform.lossyScale
        };

        TransformChange.StartTransformChange(gameObject, nonPointerMiddlePoint, totalTime / 2f, .3f, false, true, null, false);

        NoPointerTransform nonPointerDestination = new NoPointerTransform()
        {
            position = destination,
            rotation = transform.rotation,
            scale = transform.lossyScale
        };
        TransformChange.StartTransformChange(gameObject, nonPointerDestination, totalTime / 2f, 0f, true, true, null, false);
        transform.localScale = new Vector3(0, 0, 0);
    }
}
