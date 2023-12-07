using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeLayers : MonoBehaviour
{
    public Camera _MainCamera;
    public Camera _RenderTextureCamera;
    public Camera _FadedCamera;

    public RawImage _RenderTexture;

    public LayerMask _FadedLayerMask;

    private bool _Reverse;

    private float _Time = 3f;
    private float _Counter;


    public void StartFade( float time, bool reverse )
    {
        _Reverse = reverse;
        _Time = time;

        if (_Reverse)
            _Counter = _Time;
        else
            _Counter = 0;

        enabled = true;
    }

    private void OnEnable()
    {
        if ( ! _Reverse )
            StartFade();
        else
            StartUnfade();
    }

    private void Update()
    {
        FadeProgression();

        _Counter += (_Reverse ? -1 : 1) * Time.deltaTime;

        if ( ! _Reverse )
        {
            if ( _Counter >= _Time )
                EndFade();
        }
        else
        {
            if (_Counter <= 0)
                EndUnfade();
        }
    }

    #region DO THE FADE
    void StartFade()
    {
        _RenderTextureCamera.gameObject.SetActive(true);
        _RenderTexture.gameObject.SetActive(true);
        _RenderTexture.color = new Color(_RenderTexture.color.r, _RenderTexture.color.g, _RenderTexture.color.b, 0f);
    }

    void EndFade()
    {
        _RenderTextureCamera.gameObject.SetActive(false);
        _RenderTexture.gameObject.SetActive(false);

        _FadedCamera.gameObject.SetActive(true);

        this.enabled = false;
    }
    #endregion


    void FadeProgression()
    {
        _RenderTexture.color = new Color(_RenderTexture.color.r, _RenderTexture.color.g, _RenderTexture.color.b, _Counter / _Time);
    }


    #region UNDO THE FADE
    void StartUnfade()
    {
        _RenderTextureCamera.gameObject.SetActive(true);
        _RenderTexture.gameObject.SetActive(true);
        _RenderTexture.color = new Color(_RenderTexture.color.r, _RenderTexture.color.g, _RenderTexture.color.b, 1f);

        _FadedCamera.gameObject.SetActive(false);
    }

    void EndUnfade()
    {
        _RenderTextureCamera.gameObject.SetActive(false);
        _RenderTexture.gameObject.SetActive(false);

        this.enabled = false;
    }
    #endregion
}
