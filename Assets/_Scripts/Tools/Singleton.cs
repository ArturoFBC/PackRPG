using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T reference;
    public static T Ref
    {
        get
        {
            if (reference == null)
            {
                reference = FindObjectOfType<T>();
                if (reference != null)
                    reference.InheritedAwake();
            }

            return reference;
        }

        private set
        {
            reference = value;
        }
    }

    protected void Awake()
    {
        if (reference == null)
        {
            reference = (T)this;
        }
        else if (reference != this)
        {
            Destroy(this);
            return;
        }

        InheritedAwake();
    }

    protected virtual void InheritedAwake() {}
}
