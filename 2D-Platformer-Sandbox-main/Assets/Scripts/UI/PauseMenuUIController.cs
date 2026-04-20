using System;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class PauseMenuUIController : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _pauseMenuPanel;
        
        private Button _resumeButton;
        private Button _backToMainMenuButton;
        private Button _quitButton;
        
        private void OnEnable()
        {
            // Get the root of the UI document
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            // Query UI elements
            _pauseMenuPanel = _root.Q<VisualElement>("PauseMenuPanel");
            _resumeButton = _root.Q<Button>("ResumeButton");
            _backToMainMenuButton = _root.Q<Button>("BackToMainMenuButton");
            _quitButton = _root.Q<Button>("QuitButton");
            
            if (_pauseMenuPanel == null ||
                _resumeButton == null ||
                _backToMainMenuButton == null ||
                _quitButton == null)
            {
                Debug.LogError("PauseMenuUIController: One or more UI elements not found in UXML.");
                return;
            }

            // Button callbacks
            _resumeButton.clicked += OnResumeClicked;
            _backToMainMenuButton.clicked += OnBackToMainMenuClicked;
            _quitButton.clicked += OnQuitClicked;
            
            // Subscribe to pause toggled event
            ManagerRoot.Instance.GamePauseManager.OnPauseToggled += OnPauseToggled;
            
            // Sync initial state
            OnPauseToggled(ManagerRoot.Instance.GamePauseManager.IsPaused);
        }
        
        private void OnDisable()
        {
            if (ManagerRoot.Instance?.GamePauseManager != null)
                ManagerRoot.Instance.GamePauseManager.OnPauseToggled -= OnPauseToggled;

            if (_resumeButton != null)
                _resumeButton.clicked -= OnResumeClicked;

            if (_backToMainMenuButton != null)
                _backToMainMenuButton.clicked -= OnBackToMainMenuClicked;

            if (_quitButton != null)
                _quitButton.clicked -= OnQuitClicked;
        }

        private void OnPauseToggled(bool isPaused)
        {
            _pauseMenuPanel.style.display =
                isPaused ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void OnResumeClicked()
        {
            ManagerRoot.Instance.GamePauseManager.TogglePause();
        }
        
        private void OnBackToMainMenuClicked()
        {
            ManagerRoot.Instance.GameSceneManager.LoadMainMenuScene();
        }
        
        private void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
