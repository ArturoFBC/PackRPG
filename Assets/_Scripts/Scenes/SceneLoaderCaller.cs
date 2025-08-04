using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pack.Scripts.Scenes
{
    public class SceneLoaderCaller : MonoBehaviour
    {
        [SerializeField] private SceneReference _sceneToLoad;

        public void LoadScene()
        {
            SceneLoader.Ref.GoToScene(_sceneToLoad);

        }
    }
}