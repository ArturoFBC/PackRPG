using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public enum SceneType
{
    MENU,
    LAIR,
	AREA,
    STARTER_SELECTION,
    END
}

public class SceneLoader : MonoBehaviour {

	public static SceneLoader _Reference;

	private bool _Loading = false;
    public float _LoadTime = 1f;
    public float _UnloadTime = 1f;

	private SceneType _Scene;
    public SceneReference _LairReference;
    public SceneReference _MainMenuReference;
    public SceneReference _StarterSelectionReference;

	private GameObject _TransitionPanelInOut;
	
    void Awake()
    {
        if (_Reference == null)
            _Reference = this;
        else
        {
            DestroyImmediate(this);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoadedWithParams;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedWithParams;
    }

    public void OnSceneLoadedWithParams( Scene scene, LoadSceneMode mode )
	{
        OnSceneLoaded();
    }

    void OnSceneLoaded()
    {
        if (_TransitionPanelInOut == null)
            _TransitionPanelInOut = GameObject.Find("TransitionInOut");

        SetLoading(false);   
    }

    private void SetLoading(bool loading)
    {
        _Loading = loading;
    }

	public void GoToArea()
	{	
		if ( ! _Loading )
		{
            SetLoading(true);
            _Scene = SceneType.AREA;
            StartCoroutine(LoadLevel());
		}	
	}
	
	public void GoToLair()
	{
        if ( ! _Loading )
		{
            SetLoading(true);
            _Scene = SceneType.LAIR;
            StartCoroutine(LoadLevel());
		}
	}

    public void GoToStarterSelection()
    {
        if (!_Loading)
        {
            SetLoading(true);
            _Scene = SceneType.STARTER_SELECTION;
            StartCoroutine(LoadLevel());
        }
    }

    public void GoToMenu()
	{
		if ( ! _Loading )
		{
            SetLoading(true);
            _Scene = SceneType.MENU;
            StartCoroutine(LoadLevel());
		}
	}

    public void CloseApplication()
    {
        Application.Quit();
    }
	
	float StartTransition()
	{
		if ( _TransitionPanelInOut != null )
		{
			_TransitionPanelInOut.SetActive(true);

			return _UnloadTime;
		}
		return 0f;
	}

    //To be called after a delay so the transition panel can fade in.
    IEnumerator LoadLevel()
    {
        yield return new WaitForSecondsRealtime(StartTransition());

        switch (_Scene)
        {
            case SceneType.AREA:
                SceneManager.LoadScene( DataManager._CurrentArea.scene.ScenePath );
            break;
            case SceneType.LAIR:
                SceneManager.LoadScene(_LairReference);
                break;
            case SceneType.MENU:
                SceneManager.LoadScene(_MainMenuReference);
                break;
            case SceneType.STARTER_SELECTION:
                SceneManager.LoadScene(_StarterSelectionReference);
                break;
        }
	}
	
}
