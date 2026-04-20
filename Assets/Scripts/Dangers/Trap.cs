using Managers;
using UnityEngine;

namespace Dangers
{
    public class Trap : MonoBehaviour
    {
        [Tooltip("The amount of damage done to the player")]
        [SerializeField] private float damage = 50f;
        [Tooltip("Determines if the trap is active and can trigger if the Player collides with it")]
        [SerializeField] private bool isActive = true;
        
        Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isActive)
            {
                if (other.CompareTag("Player"))
                {
                    isActive = false;
                    
                    ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.Trap);
                    
                    _animator.SetTrigger("TrapTriggered");
                    
                    ManagerRoot.Instance.PlayerManager.TakeDamage(damage);
                }
            }
        
        }
    }
}
