using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Students.Luca.Scripts
{
    public class GameMaster : SerializedMonoBehaviour
    {
        public enum ScreenId
        {
            Menu,
            Level1
        }

        [OdinSerialize]
        public Dictionary<ScreenId, string> screenScenes = new Dictionary<ScreenId,string>();
        public ScreenId currentScreen;
        public GameObject loadingScreenPrefab;
        public float minLoadingTime = 1;
    
        [ShowInInspector]
        private GameObject _loadingScreen;
        private bool _currentlyLoadingScene = false;
    
        // Start is called before the first frame update
        private void Start()
        {
            ToggleLoadingScreen(false);
            DontDestroyOnLoad(gameObject);
        }
    
        public bool LoadScreen(ScreenId screenId)
        {
            if (_currentlyLoadingScene)
                return false;
            StartCoroutine(LoadScene(screenId));
            return true;
        }

        private IEnumerator LoadScene(ScreenId screenId)
        {
            // Info: Deliberately allowing same scene to be loaded again -> Simple Reload.
            if(_currentlyLoadingScene || !screenScenes.TryGetValue(screenId, out var sceneName))
                yield break;
        
            _currentlyLoadingScene = true;
            ToggleLoadingScreen(true);

            /*// Unload Current Scene
        AsyncOperation asyncOpUnloadScene = null;
        if (screenScenes.TryGetValue(currentScreen, out var oldScene))
        {
            var oldSceneFullyUnloaded = false;
            asyncOpUnloadScene = SceneManager.UnloadSceneAsync(oldScene);
            asyncOpUnloadScene.completed += op => { oldSceneFullyUnloaded = true;};

            // Hacky: No break condition if something goes wrong.
            while (!oldSceneFullyUnloaded)
            {
                yield return new WaitForEndOfFrame();
            }
        }*/
        
            yield return new WaitForSeconds(minLoadingTime); // Hack
            // Load New Scene
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncOp.allowSceneActivation = true;
            
            asyncOp.completed += op =>
            {
                ToggleLoadingScreen(false);
                currentScreen = screenId;
                _currentlyLoadingScene = false;
            };
            yield return 0;
        }

        private void ToggleLoadingScreen(bool state)
        {
            if(_loadingScreen == null && loadingScreenPrefab == null)
                return;

            if (_loadingScreen == null)
            {
                _loadingScreen = Instantiate(loadingScreenPrefab, transform);
                DontDestroyOnLoad(_loadingScreen);
            }
        
            _loadingScreen.SetActive(state);
        }
    }
}
