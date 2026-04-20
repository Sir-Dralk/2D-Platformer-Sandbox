using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace Managers
{
    public class InputTracker : MonoBehaviour
    {
        public enum InputType
        {
            MouseKeyboard,
            Controller
        }

        public InputType LastInputType { get; private set; } = InputType.MouseKeyboard;
        public bool IsActive { get; private set; }

        private HashSet<string> _cursorVisibleScenes;
        private Vector2 _lastMousePosition;
        private bool _allowCursorHide = true;
        
        private void Awake()
        {
            if (Mouse.current != null)
                _lastMousePosition = Mouse.current.position.ReadValue();
            
            _cursorVisibleScenes = new HashSet<string>(ManagerRoot.Instance.GameSceneManager.GetNonPausableScenesByName());
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _allowCursorHide = !_cursorVisibleScenes.Contains(scene.name);

            if (_allowCursorHide)
                LastInputType = InputType.Controller;

            ApplyCursorVisibility();
        }
        
        public void Activate()
        {
            IsActive = true;
            ApplyCursorVisibility();
        }
        
        public void Deactivate()
        {
            IsActive = false;
            ApplyCursorVisibility();
        }
        
        private void Update()
        {
            if (!IsActive)
                return;
            
            // Mouse movement
            if (Mouse.current != null)
            {
                Vector2 currentMousePos = Mouse.current.position.ReadValue();
                if (currentMousePos != _lastMousePosition)
                {
                    _lastMousePosition = currentMousePos;
                    SetMouseKeyboardInput();
                }
            }
            
            // Keyboard input
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                SetMouseKeyboardInput();
            }
        
            // Controller input
            if (Gamepad.current != null)
            {
                foreach (var control in Gamepad.current.allControls)
                {
                    if (control is ButtonControl button && button.wasPressedThisFrame)
                    {
                        SetControllerInput();
                        break;
                    }
                }
            }
        }
        
        public void SetMouseKeyboardInput()
        {
            if (LastInputType == InputType.MouseKeyboard) return;

            LastInputType = InputType.MouseKeyboard;
            ApplyCursorVisibility();
        }

        public void SetControllerInput()
        {
            if (LastInputType == InputType.Controller) return;

            LastInputType = InputType.Controller;
            ApplyCursorVisibility();
        }
        
        private void ApplyCursorVisibility()
        {
            bool isPaused = ManagerRoot.Instance.GamePauseManager?.IsPaused ?? false;

            bool shouldShow = !_allowCursorHide || isPaused;

            Cursor.visible = shouldShow;
            Cursor.lockState = CursorLockMode.None; // never lock
        }
    }
}
