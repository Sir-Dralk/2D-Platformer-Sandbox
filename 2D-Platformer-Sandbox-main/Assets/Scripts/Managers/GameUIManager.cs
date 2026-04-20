using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameUIManager : MonoBehaviour
    {
        [Tooltip("GameObject that holds the Main Menu UI Document")]
        [SerializeField] private GameObject mainMenuUI;
        [Tooltip("GameObject that holds the Game Scene UI Document")]
        [SerializeField] private GameObject gameSceneUI;
        [Tooltip("GameObject that holds the Win Menu UI Document")]
        [SerializeField] private GameObject winMenuUI;
        [Tooltip("GameObject that holds the Game Over Menu UI Document")]
        [SerializeField] private GameObject gameOverMenuUI;
        
        private Dictionary<string, GameObject> _sceneToUIMap;
        
        public MainMenuUIController MainMenuUIController { get; private set; }
        public GameSceneUIController GameSceneUIController { get; private set; }
        public WinMenuUIController WinMenuUIController { get; private set; }
        public GameOverMenuUIController GameOverSceneUIController { get; private set; }

        private void Awake()
        {
            MainMenuUIController = mainMenuUI.GetComponent<MainMenuUIController>();
            GameSceneUIController = gameSceneUI.GetComponent<GameSceneUIController>();
            WinMenuUIController = winMenuUI.GetComponent<WinMenuUIController>();
            GameOverSceneUIController = gameOverMenuUI.GetComponent<GameOverMenuUIController>();

            if (GameSceneUIController == null)
            {
                Debug.Log("GameUIManager: GameSceneUIController is null");
            }
            
            
            // Map scene names to UI GameObjects
            _sceneToUIMap = new Dictionary<string, GameObject>
            {
                { ManagerRoot.Instance.GameSceneManager.GetMainMenuSceneName(), mainMenuUI },
                { ManagerRoot.Instance.GameSceneManager.GetWinMenuSceneName(), winMenuUI },
                { ManagerRoot.Instance.GameSceneManager.GetGameOverMenuSceneName(), gameOverMenuUI }
            };
            
            // Map game scenes
            foreach (var sceneName in ManagerRoot.Instance.GameSceneManager.GetGameSceneNames())
            {
                _sceneToUIMap[sceneName] = gameSceneUI;
            }
            
            // Subscribe to sceneLoaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void Start()
        {
            // Set UI for the currently active scene
            UpdateUI(SceneManager.GetActiveScene().name);
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateUI(scene.name);
        }

        private void UpdateUI(string sceneName)
        {
            // Deactivate all first
            mainMenuUI.SetActive(false);
            gameSceneUI.SetActive(false);
            winMenuUI.SetActive(false);
            gameOverMenuUI.SetActive(false);

            // Activate the right UI if mapped
            if (_sceneToUIMap.TryGetValue(sceneName, out var ui))
            {
                ui.SetActive(true);
            }
        }
        
        // Optional helper methods for external calls
        public void HideAllUI()
        {
            mainMenuUI.SetActive(false);
            gameSceneUI.SetActive(false);
            gameOverMenuUI.SetActive(false);
        }
    }
}
