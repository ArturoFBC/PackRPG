using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneButton : MonoBehaviour
{
    public void GoToArea(Area destinationArea)
    {
        DataManager.SetArea(destinationArea);

        SceneLoader._Reference.GoToArea();
    }

    public void GoToLair()
    {
        SceneLoader._Reference.GoToLair();
    }

    public void GoToMenu()
    {
        SceneLoader._Reference.GoToMenu();
    }

    public void GoToStarterSelection()
    {
        SceneLoader._Reference.GoToStarterSelection();
    }

    public void QuitGame()
    {
        SceneLoader._Reference.CloseApplication();
    }
}
