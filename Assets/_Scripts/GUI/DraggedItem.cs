using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggedItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    private RectTransform myTransform;
    private Canvas myCanvas;

    private void Awake()
    {
        if (myCanvas == null)
            myCanvas = GetComponentInParent<Canvas>();

        if (icon == null)
            icon = GetComponent<Image>();

        myTransform = GetComponent<RectTransform>();
    }

    public void SetImage( Sprite sprite )
    {
        icon.sprite = sprite;
    }

    private void LateUpdate()
    {
        myTransform.position =
                myCanvas.renderMode == RenderMode.ScreenSpaceOverlay ?
                Input.mousePosition + (Vector3)GetOffset() :
                Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, myCanvas.planeDistance) + (Vector3)GetOffset());
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
