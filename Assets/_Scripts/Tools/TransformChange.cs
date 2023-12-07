using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public struct NoPointerTransform
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

public class TransformChangeInfo
{
    public NoPointerTransform transform = new NoPointerTransform();
    public float time;
    public float delay;
    public bool exponential = false;
    public bool world = false;
    //public Sound sound = Sound.END;
    public Transform destinationParent = null;
    public bool cancelQueue = false;
}

public class TransformChange : MonoBehaviour
{

    NoPointerTransform _InitialTransform = new NoPointerTransform();
    NoPointerTransform _FinalTransform = new NoPointerTransform();
    float _Counter;
    float _TotalTime;
    float _Delay;
    bool _Exponential = false;
    bool _World = false;
    Transform _DestinationParent = null;
    Queue<TransformChangeInfo> _ChangeQueue = new Queue<TransformChangeInfo>();

    void Awake()
    {
        if (transform.localRotation == _FinalTransform.rotation &&
             transform.localPosition == _FinalTransform.position &&
             transform.localScale == _FinalTransform.scale)
        {
            this.enabled = false;
        }
    }


    void Update()
    {
        if (_Delay > 0)
        {
            _Delay -= Time.deltaTime;
        }
        else if (transform.localRotation != _FinalTransform.rotation ||
             transform.localPosition != _FinalTransform.position ||
             transform.localScale != _FinalTransform.scale)
        {
            if (_Counter > 0)
            {
                float progression = (_TotalTime - _Counter) / _TotalTime;
                if (_Exponential)
                    progression *= progression;

                if (_World)
                {
                    transform.rotation = Quaternion.Lerp(_InitialTransform.rotation, _FinalTransform.rotation, progression);
                    transform.position = Vector3.Lerp(_InitialTransform.position, _FinalTransform.position, progression);
                    transform.localScale = Vector3.Lerp(_InitialTransform.scale, _FinalTransform.scale, progression);
                }
                else
                {
                    transform.localRotation = Quaternion.Lerp(_InitialTransform.rotation, _FinalTransform.rotation, progression);
                    transform.localPosition = Vector3.Lerp(_InitialTransform.position, _FinalTransform.position, progression);
                    transform.localScale = Vector3.Lerp(_InitialTransform.scale, _FinalTransform.scale, progression);
                }

                _Counter -= Time.deltaTime;
            }
            else
            {
                if (_World)
                {
                    if (_DestinationParent != null)
                        transform.parent = _DestinationParent;

                    transform.rotation = _FinalTransform.rotation;
                    transform.position = _FinalTransform.position;
                    transform.localScale = _FinalTransform.scale;
                }
                else
                {
                    if (_DestinationParent != null)
                        transform.parent = _DestinationParent;

                    transform.localRotation = _FinalTransform.rotation;
                    transform.localPosition = _FinalTransform.position;
                    transform.localScale = _FinalTransform.scale;
                }
            }
        }
        else
        {
            //Movement end	
            this.enabled = false;
            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = true;

            //Check if there are queued movements
            if (_ChangeQueue.Count > 0)
                NonStaticStartTransformChange(_ChangeQueue.Dequeue());
        }
    }

    public void NonStaticStartTransformChange(TransformChangeInfo tci)
    {
        if (this.enabled && tci.cancelQueue == false)
        {
            _ChangeQueue.Enqueue(tci);
        }
        else
        {
            this.enabled = true;
            _FinalTransform = tci.transform;
            _InitialTransform = new NoPointerTransform();
            _DestinationParent = tci.destinationParent;

            if (tci.world)
            {
                _InitialTransform.position = transform.position;
                _InitialTransform.rotation = transform.rotation;
                _InitialTransform.scale = transform.lossyScale;
            }
            else
            {
                _InitialTransform.position = transform.localPosition;
                _InitialTransform.rotation = transform.localRotation;
                _InitialTransform.scale = transform.localScale;
            }
            _Counter = tci.time;
            _TotalTime = tci.time;
            _Exponential = tci.exponential;
            _World = tci.world;
            _Delay = tci.delay;

            if (GetComponent<Collider>() != null)
                GetComponent<Collider>().enabled = false;
        }
    }


    public static void StartTransformChange(GameObject receiver, NoPointerTransform destinationNoPointerTransform, float time, float delay, bool exponential, bool world, Transform destinationParent, bool cancelQueue)
    {
        TransformChangeInfo tci = new TransformChangeInfo();
        tci.transform = destinationNoPointerTransform;
        tci.time = time;
        tci.delay = delay;
        tci.exponential = exponential;
        tci.world = world;
        tci.destinationParent = destinationParent;
        tci.cancelQueue = cancelQueue;
        receiver.SendMessage("NonStaticStartTransformChange", tci, SendMessageOptions.DontRequireReceiver);
    }

    public static void StartTransformChange(GameObject receiver, NoPointerTransform destinationNoPointerTransform, float time)
    {
        StartTransformChange(receiver, destinationNoPointerTransform, time, 0f, false, false, null, false);
    }
}