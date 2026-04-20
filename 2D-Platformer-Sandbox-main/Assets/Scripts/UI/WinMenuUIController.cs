using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class WinMenuUIController : MonoBehaviour
    {
        private VisualElement _root;
        
        private Button _retryButton;
        private Button _backToMainMenuButton;
        private Button _quitButton;
        
        private Label _collectableCountLabel;
    
        private void OnEnable()
        {
            // Get the root of the UI document
            _root = GetComponent<UIDocument>().rootVisualElement;
            
            // Schedule UI queries for the next frame
            _root.schedule.Execute(_ =>
            {
                // Query buttons and label
                _retryButton = _root.Q<Button>("RetryButton");
                _backToMainMenuButton = _root.Q<Button>("BackToMainMenuButton");
                _quitButton = _root.Q<Button>("QuitButton");
                _collectableCountLabel = _root.Q<Label>("CollectableCountValueLabel");

                // Subscribe to player events
                var playerManager = ManagerRoot.Instance.PlayerManager;

                // Immediately update label with current value
                UpdateCollectableCountLabel(playerManager.CollectableCount);

                // Register button callbacks
                _retryButton.clicked += OnRetryClicked;
                _backToMainMenuButton.clicked += OnBackToMainMenuClicked;
                _quitButton.clicked += OnQuitClicked;
            });
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
            
            if (_collectableCountLabel != null) _collectableCountLabel.text = "";
        }
        
        public void UpdateCollectableCountLabel(int value)
        {
            if (_collectableCountLabel != null)
                _collectableCountLabel.text = value.ToString();
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
