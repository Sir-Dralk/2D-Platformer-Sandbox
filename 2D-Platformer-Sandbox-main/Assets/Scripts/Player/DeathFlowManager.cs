using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class DeathFlowManager : MonoBehaviour
    {
        [Tooltip("The Player Prefab used to instantiate the Player")]
        [SerializeField] private GameObject playerPrefab;

        private PlayerManager _playerManager;
        private Transform _spawnPoint;
        private GameObject _currentPlayer;
        private HashSet<string> _gameScenes;

        private void Awake()
        {
            _playerManager = ManagerRoot.Instance.PlayerManager;
        
            _gameScenes = new HashSet<string>(
                ManagerRoot.Instance.GameSceneManager.GetGameSceneNames()
            );
        
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // reset state every scene load
            _currentPlayer = null;
            _spawnPoint = null;

            if (!IsGameplayScene(scene.name))
                return;

            SpawnPlayer();
        }
    
        private bool IsGameplayScene(string sceneName)
        {
            return _gameScenes.Contains(sceneName);
        }
    
        private void SpawnPlayer()
        {
            // Check if a player already exists in the scene
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

            if (existingPlayer != null)
            {
                _currentPlayer = existingPlayer;
                return;
            }

            if (_spawnPoint == null)
            {
                GameObject sp = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
                if (sp != null && sp.transform.childCount > 0)
                {
                    _spawnPoint = sp.transform.GetChild(0);
                }
                else
                {
                    Debug.LogError("Spawn point not found!");
                    return;
                }
            }

            _currentPlayer = Instantiate(playerPrefab, _spawnPoint.position, _spawnPoint.rotation);
        }

        public void HandlePlayerDeath()
        {
            string sceneName = SceneManager.GetActiveScene().name;

            if (!IsGameplayScene(sceneName))
                return;
        
            // Destroy current player FIRST
            if (_currentPlayer != null)
            {
                Destroy(_currentPlayer);
                _currentPlayer = null;
            }

            if (_playerManager.LivesCount > 0)
            {
                Respawn();
            }
            else
            {
                GameOver();
            }
        }

        private void Respawn()
        {
            _playerManager.ResetHealth();

            StartCoroutine(RespawnNextFrame());
        }
        
        private IEnumerator RespawnNextFrame()
        {
            // ✅ wait until old player is actually destroyed
            yield return null;

            SpawnPlayer();
        }

        private void GameOver()
        {
            if (_currentPlayer != null)
                Destroy(_currentPlayer);
            
            ManagerRoot.Instance.GameSceneManager.LoadGameOverMenuScene();
        }
    }
}
