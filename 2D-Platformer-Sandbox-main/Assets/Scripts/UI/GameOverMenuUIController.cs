using System;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class GameOverMenuUIController : MonoBehaviour
    {
        private VisualElement _root;
        
        private Button _retryButton;
        private Button _backToMainMenuButton;
        private Button _quitButton;
    
        private void OnEnable()
        {
            // Get the root of the UI document
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            // Query buttons by name
            _retryButton = _root.Q<Button>("RetryButton");
            _backToMainMenuButton = _root.Q<Button>("BackToMainMenuButton");
            _quitButton = _root.Q<Button>("QuitButton");
            
            if (_retryButton == null ||
                _backToMainMenuButton == null ||
                _quitButton == null)
            {
                Debug.LogError("GameOverMenuUIController: One or more buttons not found in UXML.");
                return;
            }

            // Register click callbacks
            _retryButton.clicked += OnRetryClicked;
            _backToMainMenuButton.clicked += OnBackToMainMenuClicked;
            _quitButton.clicked += OnQuitClicked;
        }
        
        /// <summary>
        /// called when switching scenes or quitting application
        /// </summary>
        private void OnDisable()
        {
            if (_retryButton != null)
                _retryButton.clicked -= OnRetryClicked;

            if (_backToMainMenuButton != null)
                _backToMainMenuButton.clicked -= OnBackToMainMenuClicked;

            if (_quitButton != null)
                _quitButton.clicked -= OnQuitClicked;
        }
        
        private void OnRetryClicked()
        {
            CallReset();
            ManagerRoot.Instance.GameSceneManager.LoadGameScene(0);
        }
        
        private void OnBackToMainMenuClicked()
        {
            CallReset();
            ManagerRoot.Instance.GameSceneManager.LoadMainMenuScene();
        }

        private void OnQuitClicked()
        {
            Application.Quit();
        }

        private void CallReset()
        {
            ManagerRoot.Instance.PlayerManager.ResetValues();
        }
    }
}
