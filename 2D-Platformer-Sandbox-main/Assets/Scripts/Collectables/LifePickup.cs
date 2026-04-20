using Managers;
using UnityEngine;

public class LifePickup : MonoBehaviour
{
    [Tooltip("How many lives the pickup is worth")]
    [SerializeField] private int Amount = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
            
        var playerManager = ManagerRoot.Instance.PlayerManager;

        // Heal Player
        playerManager.ChangeLivesCount(Amount);
        
        ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.LifePickup);
        Destroy(gameObject);
    }
}
