using System.Collections;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [Tooltip("How much max health the player can have")]
        [SerializeField] private float maxHealth = 100f;
        
        [Tooltip("How many lives the player starts with")]
        [SerializeField] private int livesCount = 3;
    
        [Tooltip("How many collectibles the player starts with")]
        [SerializeField] private int collectableCount = 0;
    
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public int LivesCount { get; private set; }
        public int CollectableCount { get; private set; }
        
        private bool _isDead = false;
        private bool _deathTriggered = false;
        
        private void Awake()
        {
            // Initialize once before any Start() methods run
            ResetValues();
        }

        private void Start()
        {
            ManagerRoot.Instance.GameUIManager.GameSceneUIController.UpdateHealthBar(CurrentHealth, MaxHealth);
            ManagerRoot.Instance.GameUIManager.WinMenuUIController.UpdateCollectableCountLabel(CollectableCount);
            ManagerRoot.Instance.GameUIManager.GameSceneUIController.UpdateLivesCountUI(LivesCount);
        }

        public void TakeDamage(float value)
        {
            // HARD LOCK (prevents ANY re-entry)
            if (_isDead || _deathTriggered)
                return;
            
            CurrentHealth -= value;
            
            if (CurrentHealth > 0)
            {
                ManagerRoot.Instance.GameUIManager.GameSceneUIController
                    .UpdateHealthBar(CurrentHealth, MaxHealth);
                return;
            }

            // ENTER DEATH STATE IMMEDIATELY
            _isDead = true;
            _deathTriggered = true;

            CurrentHealth = 0;
            LivesCount--;

            ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.TakeDamage);

            ManagerRoot.Instance.GameUIManager.GameSceneUIController
                .UpdateLivesCountUI(LivesCount);

            StartCoroutine(HandleDeathNextFrame());
        }
        
        private IEnumerator HandleDeathNextFrame()
        {
            yield return null;

            ManagerRoot.Instance.DeathFlowManager.HandlePlayerDeath();
        }
        
        public bool Heal(float amount)
        {
            if (_isDead) return false;
            
            if (CurrentHealth >= MaxHealth)
                return false; // nothing to heal

            float oldHealth = CurrentHealth;

            CurrentHealth += amount;

            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;

            ManagerRoot.Instance.GameUIManager.GameSceneUIController.UpdateHealthBar(CurrentHealth, MaxHealth);

            return CurrentHealth > oldHealth; // ✅ actually healed
        }
        
        public void ChangeCollectableCount(int value)
        {
            CollectableCount += value;
            ManagerRoot.Instance.GameUIManager.GameSceneUIController.UpdateCollectibleCountUI(CollectableCount);
            ManagerRoot.Instance.GameUIManager.WinMenuUIController.UpdateCollectableCountLabel(CollectableCount);
        }

        public void ChangeLivesCount(int value)
        {
            LivesCount += value;
            ManagerRoot.Instance.GameUIManager.GameSceneUIController.UpdateLivesCountUI(LivesCount);
        }

        public void ResetHealth()
        {
            _isDead = false;
            _deathTriggered = false;
            
            CurrentHealth = MaxHealth;
            ManagerRoot.Instance.GameUIManager.GameSceneUIController.UpdateHealthBar(CurrentHealth, MaxHealth);
        }

        public void ResetValues()
        {
            _isDead = false;
            _deathTriggered = false;
            MaxHealth = maxHealth;
            CurrentHealth = MaxHealth;
            LivesCount = livesCount;
            CollectableCount = collectableCount;
        }
    }
}
