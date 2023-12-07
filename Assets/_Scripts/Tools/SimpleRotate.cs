using UnityEngine;
using System.Collections;

public class SimpleRotate : MonoBehaviour {

	public Vector3 _Axis;
	public float _Speed;

	// Update is called once per frame
	void Update ()
	{
		transform.Rotate (_Axis, _Speed * Time.deltaTime);
	}
}
