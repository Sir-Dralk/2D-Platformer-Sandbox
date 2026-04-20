using Managers;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class GameSceneUIController : MonoBehaviour
    {
        [SerializeField]  private UIDocument uiDocument;
        
        private Label _collectableValueLabel;
        private Label _livesValueLabel;

        private VisualElement _playerHealthFill;
        
        private PlayerManager _playerManager;
    
        private void Awake()
        {
            // Subscribe to player events
            _playerManager = ManagerRoot.Instance.PlayerManager;

            // Listen for scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Bind labels for the first loaded scene
            BindLabels();
        }
        
        private void BindLabels()
        {
            // Always clear old references first
            _collectableValueLabel = null;
            _livesValueLabel = null;

            if (uiDocument == null)
            {
                Debug.LogWarning("GameSceneUIController: No UIDocument assigned.");
                return;
            }

            // Wait until the UIDocument has built its visual tree
            uiDocument.rootVisualElement.schedule.Execute(_ =>
            {
                _collectableValueLabel = uiDocument.rootVisualElement.Q<Label>("CollectableValueLabel");
                _livesValueLabel = uiDocument.rootVisualElement.Q<Label>("LivesValueLabel");
                
                _playerHealthFill = uiDocument.rootVisualElement.Q<VisualElement>("HealthBarFill");

                // If gameplay labels or health bar are missing, skip updating
                if (_collectableValueLabel == null && _livesValueLabel == null && _playerHealthFill == null)
                    return;

                // Update HUD immediately when labels are found
                UpdateCollectibleCountUI(_playerManager.CollectableCount);
                UpdateLivesCountUI(_playerManager.LivesCount);
                UpdateHealthBar(_playerManager.CurrentHealth, _playerManager.MaxHealth);
            });
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Get the list of gameplay scene names
            var gameplayScenes = ManagerRoot.Instance.GameSceneManager.GetGameSceneNames();

            // Only bind labels if this scene is in the gameplay list
            if (gameplayScenes.Contains(scene.name))
            {
                BindLabels();
            }
            else
            {
                // Optional: clear labels so UI doesn't show stale data
                _playerHealthFill = null;
                _collectableValueLabel = null;
                _livesValueLabel = null;
            }
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (_playerHealthFill == null || maxHealth <= 0) return;

            // Compute percent of health
            float percent = Mathf.Clamp01(currentHealth / maxHealth);

            // Schedule the width update on the next frame
            _playerHealthFill.schedule.Execute(_ =>
            {
                if (_playerHealthFill.parent != null)
                {
                    float parentWidth = _playerHealthFill.parent.resolvedStyle.width;
                    _playerHealthFill.style.width = Length.Pixels(parentWidth * percent);
                }
            });
        }

        public void UpdateCollectibleCountUI(int value)
        {
            if (_collectableValueLabel != null)
                _collectableValueLabel.text = value.ToString();
        }
        
        public void UpdateLivesCountUI(int value)
        {
            if (_livesValueLabel != null)
                _livesValueLabel.text = value.ToString();
        }
    }
}
