using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Pack.Scripts.Scenes
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        private bool _Loading = false;
        public float _LoadTime = 1f;
        public float _UnloadTime = 1f;

        public SceneReference _LairReference;
        public SceneReference _MainMenuReference;
        public SceneReference _StarterSelectionReference;

        private GameObject _TransitionPanelInOut;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoadedWithParams;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoadedWithParams;
        }

        public void OnSceneLoadedWithParams(Scene scene, LoadSceneMode mode)
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

        public void GoToScene(SceneReference newScene)
        {
            SetLoading(true);
            StartCoroutine(LoadLevel(newScene));
        }

        public void GoToArea()
        {
            // CHECK IF SCENE REFERENCE IS ASSIGNED AFTER STARTING COROUTINE
            if (!_Loading)
            {
                SetLoading(true);
                StartCoroutine(LoadLevel(DataManager._CurrentArea.scene));
            }
        }

        public void GoToLair()
        {
            if (!_Loading)
            {
                SetLoading(true);
                StartCoroutine(LoadLevel(_LairReference));
            }
        }

        public void GoToStarterSelection()
        {
            if (!_Loading)
            {
                SetLoading(true);
                StartCoroutine(LoadLevel(_StarterSelectionReference));
            }
        }

        public void GoToMenu()
        {
            if (!_Loading)
            {
                SetLoading(true);
                StartCoroutine(LoadLevel(_MainMenuReference));
            }
        }

        public void CloseApplication()
        {
            Application.Quit();
        }

        private float StartTransition()
        {
            if (_TransitionPanelInOut != null)
            {
                _TransitionPanelInOut.SetActive(true);

                return _UnloadTime;
            }
            return 0f;
        }

        private IEnumerator LoadLevel(SceneReference scene)
        {
            yield return new WaitForSecondsRealtime(StartTransition());
            SceneManager.LoadScene(scene.ScenePath);
        }
    }
}