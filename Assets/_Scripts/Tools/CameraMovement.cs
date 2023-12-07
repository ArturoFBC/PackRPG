using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Vector3 offset = new Vector3(0f, 20f, -10f);
    
    public float speed = 0.5f;
	// Use this for initialization
	void Start ()
    {
        Vector3 targetPosition = GameManager.GetAveragePlayerPosition() + offset;
        transform.position = targetPosition;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 targetPosition = GameManager.GetAveragePlayerPosition() + offset;
        if ((transform.position - targetPosition).sqrMagnitude > 0.2f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed);
        }
	}
}
