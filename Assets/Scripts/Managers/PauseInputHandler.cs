using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class PauseInputHandler : MonoBehaviour
    {
        [Tooltip("The Input Action that is used to Pause the game")]
        [SerializeField] private InputActionReference pauseAction;
        
        private InputTracker _inputTracker;
        private GamePauseManager _pauseManager;


        private void Awake()
        {
            _inputTracker = ManagerRoot.Instance.InputTracker;
            _pauseManager = ManagerRoot.Instance.GamePauseManager;
        }
        
        private void OnEnable()
        {
            pauseAction.action.performed += OnPausePerformed;
            pauseAction.action.Enable();
        }

        private void OnDisable()
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            if (context.control.device is Gamepad)
            {
                _inputTracker.SetControllerInput();
            }
            else
            {
                _inputTracker.SetMouseKeyboardInput();
            }
            
            if (_pauseManager == null)
            {
                return;
            }

            _pauseManager.TogglePause();
        }
    }
}
