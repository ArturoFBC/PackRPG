using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplaceUV : MonoBehaviour {

    public Vector2 displacement;
    public string property = "_BumpMap";
    private Material _MyMaterial;
    private int _MapID;

    private void Start()
    {
        Renderer mr = GetComponent<Renderer>();
        if (mr != null)
            _MyMaterial = GetComponent<Renderer>().material;

        if (_MyMaterial == null)
            enabled = false;
        else
            _MapID = Shader.PropertyToID(property);
    }

    // Update is called once per frame
    void Update ()
    {
        _MyMaterial.SetTextureOffset( _MapID, displacement * Time.time );
	}
}
