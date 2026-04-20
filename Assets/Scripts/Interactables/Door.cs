using System;
using Managers;
using UnityEngine;

namespace Interactables
{
    public class Door : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.Door);
            
            // Load next scene
            ManagerRoot.Instance.GameSceneManager.LoadNextScene();
        }
    }
}
