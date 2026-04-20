using Managers;
using UnityEngine;

namespace Collectables
{
    public class MedPack : MonoBehaviour
    {
        [Tooltip("Amount the medpack heals the player")]
        [SerializeField] private float healAmount = 50f;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            var playerManager = ManagerRoot.Instance.PlayerManager;

            // Heal Player
            bool healed = playerManager.Heal(healAmount);

            if (healed)
            {
                ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.Heal);
                Destroy(gameObject);
            }
        }
    }
}
