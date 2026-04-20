using UnityEngine;

namespace Camera
{
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("Toggle this on for the camera to follow the player or off for it to be static")]
        [SerializeField] private bool toggleFollowPlayer = true;
    
        private Transform _target;

        [Tooltip("The offset of the camera")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

        private void Start()
        {
            FindPlayer();
        }

        private void LateUpdate()
        {
            if (toggleFollowPlayer)
            {
                if (_target == null)
                {
                    FindPlayer();
                    return;
                }

                transform.position = _target.position + offset;
            }
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                _target = player.transform;
            }
        }
    }
}
