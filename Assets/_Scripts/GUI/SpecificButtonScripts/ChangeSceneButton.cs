using Pack.Scripts.Scenes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    public void GoToArea(Area destinationArea)
    {
        DataManager.SetArea(destinationArea);

        SceneLoader.Ref.GoToArea();
    }

    public void GoToLair()
    {
        SceneLoader.Ref.GoToLair();
    }

    public void GoToMenu()
    {
        SceneLoader.Ref.GoToMenu();
    }

    public void GoToStarterSelection()
    {
        SceneLoader.Ref.GoToStarterSelection();
    }

    public void QuitGame()
    {
        SceneLoader.Ref.CloseApplication();
    }
}
