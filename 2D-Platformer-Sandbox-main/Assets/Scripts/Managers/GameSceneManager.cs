 using System.Collections;
 using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        [Header("Scene Names (must match Build Settings)")]
        [Tooltip("Name of the Main Menu Scene")]
        [SerializeField] private string mainMenuScene;
        [Tooltip("List of game scene names")]
        [SerializeField] private List<string> gameScenes;
        [Tooltip("Name of the Win Menu Scene")]
        [SerializeField] private string winMenuScene;
        [Tooltip("Name of the Game Over Menu Scene")]
        [SerializeField] private string gameOverMenuScene;

        private int _currentSceneIndex;
        private bool _isLoadingScene = false;
        
        #region Scene Name Getters
        public string GetMainMenuSceneName() => mainMenuScene;
        public string GetGameOverMenuSceneName() => gameOverMenuScene;
        public string GetWinMenuSceneName() => winMenuScene;
        public List<string> GetGameSceneNames()
        {
            return new List<string>(gameScenes);
        }
        #endregion
        
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentSceneIndex = gameScenes.IndexOf(scene.name);
            
            // Reset loading lock after scene is loaded
            _isLoadingScene = false;
        }

        public void LoadMainMenuScene()
        {
            TryLoadScene(mainMenuScene);
            // reset player values
            ManagerRoot.Instance.PlayerManager.ResetValues();
        }
        
        public void LoadWinMenuScene()
        {
            TryLoadScene(winMenuScene);
        }
        
        public void LoadGameOverMenuScene()
        {
            TryLoadScene(gameOverMenuScene);
        }
        
        public void LoadGameScene(int index)
        {
            if (index < 0 || index >= gameScenes.Count)
            {
                Debug.LogError($"Invalid scene index: {index}");
                return;
            }

            TryLoadScene(gameScenes[index]);
        }

        public void LoadNextScene()
        {
            if (gameScenes == null || gameScenes.Count == 0)
            {
                Debug.LogError("Game scenes list is empty.");
                return;
            }

            if (_currentSceneIndex < 0)
            {
                TryLoadScene(gameScenes[0]);
                return;
            }

            if (_currentSceneIndex + 1 >= gameScenes.Count)
            {
                LoadWinMenuScene();
                return;
            }

            _currentSceneIndex++;
            TryLoadScene(gameScenes[_currentSceneIndex]);
        }
        
        private void TryLoadScene(string sceneName)
        {
            if (_isLoadingScene || string.IsNullOrEmpty(sceneName))
                return;

            _isLoadingScene = true;

            StartCoroutine(LoadSceneSafely(sceneName));
        }
        
        private IEnumerator LoadSceneSafely(string sceneName)
        {
            // wait one frame (NOT affected by timeScale)
            yield return null;

            // wait end of frame (also safe)
            yield return new WaitForEndOfFrame();

            SceneManager.LoadScene(sceneName);
        }
        
        #region Utility
        public List<string> GetNonPausableScenesByName()
        {
            var result = new List<string>();

            if (!string.IsNullOrEmpty(mainMenuScene))
                result.Add(mainMenuScene);

            if (!string.IsNullOrEmpty(winMenuScene))
                result.Add(winMenuScene);

            if (!string.IsNullOrEmpty(gameOverMenuScene))
                result.Add(gameOverMenuScene);

            return result;
        }
        #endregion
    }
}
