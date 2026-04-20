using System;
using Managers;
using Player;
using UnityEngine;

namespace Collectables
{
    public class Collectable : MonoBehaviour
    {
        [Tooltip("Value of the individual collectable")]
        [SerializeField] private int collectableValue = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();

                if (player != null)
                {
                    ManagerRoot.Instance.PlayerManager.ChangeCollectableCount(collectableValue);
                    ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.CollectablePickup);
                    Destroy(gameObject);
                }
            }
        }
    }
}
