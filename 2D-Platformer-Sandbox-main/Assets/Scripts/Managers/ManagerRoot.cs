using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class ManagerRoot : MonoBehaviour
    {
        public static ManagerRoot Instance { get; private set; }

        public GameSceneManager GameSceneManager { get; private set; }
        public GameAudioManager GameAudioManager { get; private set; }
        
        public GamePauseManager GamePauseManager { get; private set; }
        
        public GameUIManager GameUIManager { get; private set; }
        
        public InputTracker InputTracker { get; private set; }
        
        public PauseInputHandler PauseInputHandler { get; private set; }
        
        public PlayerManager PlayerManager { get; private set; }
        
        public DeathFlowManager DeathFlowManager { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            GameSceneManager = GetComponentInChildren<GameSceneManager>();
            GameAudioManager = GetComponentInChildren<GameAudioManager>();
            GamePauseManager = GetComponentInChildren<GamePauseManager>();
            GameUIManager = GetComponentInChildren<GameUIManager>();
            InputTracker = GetComponentInChildren<InputTracker>();
            PauseInputHandler = GetComponentInChildren<PauseInputHandler>();
            PlayerManager = GetComponentInChildren<PlayerManager>();
            DeathFlowManager = GetComponentInChildren<DeathFlowManager>();
            
            if (GameSceneManager == null)
                Debug.LogError("Managers: GameSceneManager not found as child.");

            if (GameAudioManager == null)
                Debug.LogError("Managers: GameAudioManager not found as child.");
            
            if (GamePauseManager == null)
                Debug.LogError("Managers: GamePauseManager not found as child.");
            
            if (GameUIManager == null)
                Debug.LogError("Managers: GameUIManager not found as child.");
            
            if (InputTracker == null)
                Debug.LogError("Managers: InputTracker not found as child.");
            
            if (PauseInputHandler == null)
                Debug.LogError("Managers: PauseInputHandler not found as child.");
            
            if (PlayerManager == null)
                Debug.LogError("Managers: PlayerManager not found as child.");
            
            if (DeathFlowManager == null)
                Debug.LogError("Managers: DeathFlowManager not found as child.");
        }
    }
}
