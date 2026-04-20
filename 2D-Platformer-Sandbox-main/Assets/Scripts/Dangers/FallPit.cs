using System;
using Managers;
using Player;
using UnityEngine;

namespace Dangers
{
    public class FallPit : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            var playerManager = ManagerRoot.Instance.PlayerManager;

            // Deal lethal damage
            playerManager.TakeDamage(playerManager.MaxHealth);
        }
    }
}
