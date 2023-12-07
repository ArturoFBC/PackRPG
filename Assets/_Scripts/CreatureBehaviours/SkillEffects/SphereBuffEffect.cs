using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereBuffEffect : MonoBehaviour {

    Material _MyMaterial;
    public float _Duration;
    float _TimeLeft;
    float _FadeInTime;
    float _FadeOutTime;

    public float _MoveSpeed;

    float _BaseAlpha;
    Color _TempColor;
    Color _FullAlphaColor;
    Color _TransparentColor;

	void Awake ()
    {
        if (_MyMaterial == null)
            _MyMaterial = GetComponent<MeshRenderer>().material;

        _TempColor = _MyMaterial.GetColor("_TintColor");
        _FullAlphaColor = _TempColor;
        _BaseAlpha = _TempColor.a;
        _TransparentColor = new Color(_FullAlphaColor.r, _FullAlphaColor.g, _FullAlphaColor.b, 0f);
        _MyMaterial.SetColor("_TintColor", _TransparentColor);

        _TimeLeft = _Duration;
        _FadeInTime = 3 * _Duration / 4f;
        _FadeOutTime = _Duration / 4f;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(_TimeLeft > 0 )
        {
            _MyMaterial.SetTextureOffset( "_MainTex" , new Vector2( 0, _MoveSpeed * Time.time ));
            if ( _TimeLeft > _FadeInTime)
            { //Fade in
                _TempColor = Color.Lerp(_FullAlphaColor, _TransparentColor, (_TimeLeft - _FadeInTime) / (_Duration - _FadeInTime));
            }
            else if (_TimeLeft < _FadeOutTime)
            { //Fade out
                _TempColor = Color.Lerp(_TransparentColor, _FullAlphaColor, _TimeLeft / _FadeOutTime);
            }
            else
            {
                _TempColor.a = _BaseAlpha;
            }
            _MyMaterial.SetColor("_TintColor", _TempColor);


            _TimeLeft -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
	}
}
