using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class TransitionInOutPanel : MonoBehaviour
{
    public float _Duration = 1f;
    public float _Counter = 0f;

    private bool _FadeIn = true;

    CanvasGroup _MyCanvasGroup;



    private void Awake()
    {
        if (_MyCanvasGroup == null)
            _MyCanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        StartFade( );
    }

    private void StartFade()
    {
        SetChildrenActive(true);

        _MyCanvasGroup.alpha = _FadeIn ? 1f : 0f;
        _MyCanvasGroup.enabled = true;

        _Counter = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if ( _Counter >= _Duration )
        {
            if (_FadeIn)
            {
                _MyCanvasGroup.enabled = false;
                SetChildrenActive(false);
                gameObject.SetActive(false);
                _FadeIn = false;
            }
        }
        else
        {
            float progress = _Counter / _Duration;
            if (_FadeIn)
                progress = 1 - progress;

            _MyCanvasGroup.alpha = progress * 1.1f;
            _Counter += Time.unscaledDeltaTime;
        }
    }

    private void SetChildrenActive( bool active )
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive( active );
        }
    }
}
