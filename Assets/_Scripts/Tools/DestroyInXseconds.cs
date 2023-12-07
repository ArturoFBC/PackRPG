using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInXseconds : MonoBehaviour {

    public float timeToBeDestroyed = 1f;

	// Use this for initialization
	void Start ()
    {
        Destroy( gameObject, timeToBeDestroyed );
    }
	
}
