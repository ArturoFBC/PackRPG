using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TooltipDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject _Tooltip;
    protected GameObject _InstantiatedTooltip;
    public object _ObjectToDisplay;
    private Canvas _MyCanvas;

    public void Awake()
    {
        if (_MyCanvas == null)
            _MyCanvas = GetComponentInParent<Canvas>();
    }


    public void SetObjectToDisplay(object objectToDisplay)
    {
        _ObjectToDisplay = objectToDisplay;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_ObjectToDisplay != null)
        {
            DestroyTooltip();

            _InstantiatedTooltip = Instantiate(_Tooltip, eventData.pointerCurrentRaycast.worldPosition, transform.rotation, _MyCanvas.transform);
            

            AdjustTooltip();

            SendSetTooltip();
        }
    }

    protected abstract void SendSetTooltip();


    public void LateUpdate()
    {
        if (_InstantiatedTooltip != null)
            _InstantiatedTooltip.transform.position = 
                _MyCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? 
                Input.mousePosition + (Vector3)GetOffset(): 
                Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, _MyCanvas.planeDistance) + (Vector3)GetOffset());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyTooltip();
    }

    public void OnDisable()
    {
        DestroyTooltip();
    }

    private void DestroyTooltip()
    {
        if (_InstantiatedTooltip != null)
            Destroy(_InstantiatedTooltip);
    }

    private void AdjustTooltip()
    {
        RectTransform instantiatedToolTipTransform = _InstantiatedTooltip.GetComponent<RectTransform>();

        Vector2 newPivot = new Vector2();
        if (Input.mousePosition.x > (Screen.width / 2))
            newPivot.x = 1;
        else
            newPivot.x = 0;

        if (Input.mousePosition.y > (Screen.height / 2))
            newPivot.y = 1;
        else
            newPivot.y = 0;

        instantiatedToolTipTransform.pivot = newPivot;
    }

    private Vector2 GetOffset()
    {
        Vector2 offset = new Vector2();
        if (Input.mousePosition.x > (Screen.width / 2))
            offset.x = -1;
        else
            offset.x = 1;

        if (Input.mousePosition.y > (Screen.height / 2))
            offset.y = -1;
        else
            offset.y = 1;

        return offset;
    }
}
