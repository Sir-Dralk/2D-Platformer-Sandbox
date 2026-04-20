using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GamePauseManager : MonoBehaviour
    {
        public event Action<bool> OnPauseToggled;
        
        public bool IsPaused => _isPaused;
        
        private bool _isPaused;

        private HashSet<string> _nonPausableSceneNames;
        
        private InputTracker _inputTracker;

        private void Awake()
        {
            _inputTracker = ManagerRoot.Instance.InputTracker;
            
            _nonPausableSceneNames = new HashSet<string>(ManagerRoot.Instance.GameSceneManager.GetNonPausableScenesByName()); 
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Safety: never stay paused in a non-pausable scene
            if (!CanPauseInScene(scene.name))
                ForceUnpause();
        }
        
        public void TogglePause()
        {
            if (!CanPause())
            {
                return;
            }

            SetPaused(!_isPaused);
        }
        
        public void ForceUnpause()
        {
            if (!_isPaused)
                return;

            SetPaused(false);
        }
        
        public bool CanPause()
        {
            return CanPauseInScene(SceneManager.GetActiveScene().name);
        }
        
        private void SetPaused(bool paused)
        {
            _isPaused = paused;
            Time.timeScale = paused ? 0f : 1f;

            if (paused)
                _inputTracker.Activate();
            else
                _inputTracker.Deactivate();

            OnPauseToggled?.Invoke(paused);
        }
        
        private bool CanPauseInScene(string sceneName)
        {
            // Non-pausable scenes are explicitly listed
            return !_nonPausableSceneNames.Contains(sceneName);
        }
    }
}
