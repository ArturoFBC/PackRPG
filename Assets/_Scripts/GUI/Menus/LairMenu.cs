using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuState
{
    ANIMATION,
    READY
}

public enum LairSubMenus
{
    MAIN,
    PACK,
    MAP,
    ITEM,
    CRAFT,
    SCRAP,
    END
}

public class LairMenu : MonoBehaviour
{
    private MenuState _State;
    public MenuState State
    {
        get { return _State; }
    }

    public Transform[] _CameraPositions;
    public GameObject[] _SubMenus;

    private LairSubMenus _CurrentMenu;
    private LairSubMenus _DestinationMenu;
    private Stack<LairSubMenus> _SubMenuStack = new Stack<LairSubMenus>();

    public float _TravelingTime;
    private float _TravelingCounter;

    public Transform _Camera;

    public void GoToMenu( int lairMenu )
    {
        _SubMenuStack.Push(_CurrentMenu);
        _DestinationMenu = (LairSubMenus)lairMenu;
        _TravelingCounter = _TravelingTime;
        _State = MenuState.ANIMATION;
        _SubMenus[(int)_CurrentMenu].SetActive(false);
    }

    public void GoBack()
    {
        _DestinationMenu = _SubMenuStack.Pop();
        _TravelingCounter = _TravelingTime;
        _State = MenuState.ANIMATION;
        _SubMenus[(int)_CurrentMenu].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ( _State == MenuState.ANIMATION )
        {
            if ( _TravelingCounter <= 0 )
            {   
                // Movement end
                if (_CameraPositions.Length > (int)_DestinationMenu)
                {
                    _Camera.position = _CameraPositions[(int)_DestinationMenu].position;
                    _Camera.rotation = _CameraPositions[(int)_DestinationMenu].rotation;
                }
                _CurrentMenu = _DestinationMenu;
                _TravelingCounter = 0;
                _State = MenuState.READY;
                _SubMenus[(int)_CurrentMenu].SetActive(true);
            }
            else
            {
                // During movement
                if (_CameraPositions.Length > (int)_DestinationMenu && _CameraPositions.Length > (int)_CurrentMenu)
                {
                    float progression = Mathf.SmoothStep(0, 1, (_TravelingTime - _TravelingCounter) / _TravelingTime);

                    _Camera.position = Vector3.Lerp(_CameraPositions[(int)_CurrentMenu].position, _CameraPositions[(int)_DestinationMenu].position, progression);
                    _Camera.rotation = Quaternion.Lerp(_CameraPositions[(int)_CurrentMenu].rotation, _CameraPositions[(int)_DestinationMenu].rotation, progression);
                }
                _TravelingCounter -= Time.deltaTime;
            }
        }
    }
}
