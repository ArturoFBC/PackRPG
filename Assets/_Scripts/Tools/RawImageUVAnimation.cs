using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(RawImage))]
public class RawImageUVAnimation : MonoBehaviour
{
    public Vector2 displacement;
    private RawImage _MyRawImage;
    private int _MapID;

    private void Start()
    {
        if (_MyRawImage == null)
            _MyRawImage = GetComponent<RawImage>();

        if (_MyRawImage == null)
            enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Rect tempRect = _MyRawImage.uvRect;
        tempRect.position = displacement * Time.unscaledTime;
        _MyRawImage.uvRect = tempRect;
    }
}

