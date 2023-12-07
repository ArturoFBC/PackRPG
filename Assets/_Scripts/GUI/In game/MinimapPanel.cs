using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapPanel : MonoBehaviour
{
    public RawImage _MyMapImage;

    public RenderTexture _MiniMapRenderTexture;

    private Camera _MinimapCamera;
    private float _CameraHeight = 100;
    public float _CameraZoomSize = 10;

    private List<Image> _PlayerMarks = new List<Image>();
    [SerializeField] private float _PlayerMarkSize = 10;
    private Image _ExitMark;
    [SerializeField] private float _ExitMarkSize = 10;

    [SerializeField] private Color _SelectedPlayerColor = Color.green;
    [SerializeField] private Color _UnselectedPlayerColor = Color.yellow;
    [SerializeField] private Color _ExitMarkColor = Color.cyan;

    private void Awake()
    {
        CreateMinimapCamera();

        if (_MyMapImage == null)
            _MyMapImage = GetComponentInChildren<RawImage>();

        _MyMapImage.texture = _MiniMapRenderTexture;
    }

    private void CreateMinimapCamera()
    {
        Transform minimapTransform = (new GameObject()).transform;
        minimapTransform.gameObject.name = "MinimapCamera";
        minimapTransform.rotation = Quaternion.Euler(90f, 0f, 0f);
        minimapTransform.position = new Vector3(0f, _CameraHeight, 0f);

        _MinimapCamera = minimapTransform.gameObject.AddComponent<Camera>();
        _MinimapCamera.aspect = 1f;
        _MinimapCamera.orthographic = true;
        _MinimapCamera.targetTexture = _MiniMapRenderTexture;
        _MinimapCamera.orthographicSize = _CameraZoomSize;
        _MinimapCamera.clearFlags = CameraClearFlags.SolidColor;
        LayerMask cameraLayerCulling = LayerMask.GetMask("Terrain"); // = Camera.main.cullingMask;
        _MinimapCamera.cullingMask = cameraLayerCulling;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = GameManager.GetAveragePlayerPosition();
        targetPosition.y = _CameraHeight;
        _MinimapCamera.transform.position = targetPosition;

        Vector2 cameraPos = new Vector2(_MinimapCamera.transform.position.x, _MinimapCamera.transform.position.z);
        Vector2 minimapScreenSize = _MyMapImage.rectTransform.rect.size / 2;
        Vector2 worldUnityToPixelRatio = minimapScreenSize / _CameraZoomSize;

        for (int i = 0; i < GameManager.playerCreatures.Count; i++)
        {
            if (_PlayerMarks.Count <= i)
                _PlayerMarks.Add(CreateMinimapMark("PlayerMark", _PlayerMarkSize));

            MovePlayerMark(cameraPos, worldUnityToPixelRatio, _PlayerMarks[i], GameManager.playerCreatures[i], minimapScreenSize);
        }

        if (_ExitMark == null)
            _ExitMark = CreateMinimapMark("ExitMark", _ExitMarkSize);
        else
        {
            _ExitMark.sprite = IconsAndEffects._Ref.exitMapMark;
            _ExitMark.color = _ExitMarkColor;

            MoveMinimapMark(cameraPos, worldUnityToPixelRatio, _ExitMark, DataManager._CurrentArea.exitToNextAreaPosition, Quaternion.identity, minimapScreenSize);
        }
    }

    private void MovePlayerMark(Vector2 cameraPos, Vector2 worldUnityToPixelRatio, Image markImage, GameObject gameObjectToRepresent, Vector2 mapSize)
    {
        if (gameObjectToRepresent == GameManager.selectedCreature)
        {
            markImage.sprite = IconsAndEffects._Ref.playerMapMarkSelected;
            markImage.color = _SelectedPlayerColor;
        }
        else
        {
            markImage.sprite = IconsAndEffects._Ref.playerMapMarkUnselected;
            markImage.color = _UnselectedPlayerColor;
        }

        MoveMinimapMark(cameraPos, worldUnityToPixelRatio, markImage, gameObjectToRepresent.transform.position, gameObjectToRepresent.transform.rotation, mapSize);
    }

    private static void MoveMinimapMark(Vector2 cameraPos, Vector2 worldUnityToPixelRatio, Image markImage, Vector3 positionToRepresent, Quaternion rotationToReprensent, Vector2 mapSize)
    {
        Vector2 objectPosition = new Vector2(positionToRepresent.x, positionToRepresent.z);
        objectPosition -= cameraPos;
        objectPosition *= worldUnityToPixelRatio;
        objectPosition = KeepPositionWithinSquare( objectPosition, mapSize.x - 10f);

        markImage.rectTransform.localPosition = objectPosition;

        if (rotationToReprensent == Quaternion.identity)
            markImage.rectTransform.eulerAngles = new Vector3(0f, 0f, Vector2.Angle(Vector2.down, objectPosition));
        else
            markImage.rectTransform.eulerAngles = new Vector3(0f, 0f, 90 - rotationToReprensent.eulerAngles.y);

    }

    private Image CreateMinimapMark( string name, float markSize )
    {
        GameObject newMinimapMarkGO = new GameObject(name);
        RectTransform newMinimapMarkTransform = newMinimapMarkGO.AddComponent<RectTransform>();
        newMinimapMarkTransform.SetParent( _MyMapImage.rectTransform, false );
        newMinimapMarkTransform.localScale = Vector3.one;
        newMinimapMarkTransform.sizeDelta = new Vector2(markSize, markSize);
        newMinimapMarkTransform.SetSiblingIndex(0);

        Image newPlayerMark = newMinimapMarkGO.AddComponent<Image>();

        return newPlayerMark;
    }

    private static Vector2 KeepPositionWithinSquare( Vector2 position, float squareSize )
    {
        Vector2 cappedPosition = position;

        float biggerDimension;
        if ( Mathf.Abs(position.x) > Mathf.Abs(position.y) )
            biggerDimension = Mathf.Abs(position.x);
        else
            biggerDimension = Mathf.Abs(position.y);

        if (biggerDimension > squareSize)
        {
            float resizeFactor = Mathf.Abs(squareSize / biggerDimension);
            cappedPosition = position * resizeFactor;
        }

        return cappedPosition;
    }
}
