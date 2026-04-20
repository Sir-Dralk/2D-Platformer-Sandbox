using System;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Speed at which the Player moves")]
        [SerializeField] private float speed = 5f;
        
        [Header("Jump")]
        [Tooltip("How much velocity the Player jumps with")]
        [SerializeField] private float jumpForce = 5f;
        [Tooltip("Reduces upward velocity when releasing jump early. Lower values = shorter jumps.")]
        [SerializeField, Range(0f, 0.99f)] private float jumpCutMultiplier = 0.1f; // lower = shorter tap jump
        [Tooltip("Multiplies gravity while falling to make descents faster and less floaty.")]
        [SerializeField] private float fallGravityMultiplier = 2f;
        
        [Header("Double Jump")]
        [Tooltip("Determines if the Player is able to use Double jump")]
        [SerializeField] private bool canDoubleJump = true;
        [Tooltip("How much velocity the Player double jumps with")]
        [SerializeField] private float doubleJumpForce = 7f;
        [Tooltip("Reduces upward velocity when releasing double jump early. Lower values = shorter double jumps.")]
        [SerializeField, Range(0f, 0.99f)] private float doubleJumpCutMultiplier = 0.2f;
        
        [Header("Jump Timing")]
        [Tooltip("Time after leaving ground where a jump is still allowed (forgiveness window).")]
        [SerializeField, Range(0f, 0.5f)] private float coyoteTime = 0.15f; // seconds after leaving ground
        [Tooltip("Time to buffer a jump input before landing so it triggers on contact.")]
        [SerializeField, Range(0f, 0.5f)] private float jumpBufferTime = 0.1f; // seconds before landing
        
        [Header("Ground Detection")]
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        
        private bool _jumpStarted;
        private bool _jumpQueued;
        private bool _jumpHeld;
        
        private bool _hasDoubleJumped;
        private bool _isDoubleJumping; // tracks if the current jump is a double jump
        
        // Timing
        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;

        private SpriteRenderer _spriteRenderer;
        
        // Animator
        private Animator _anim;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }
        
        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                _jumpQueued = true;
                _jumpHeld = true;
                _jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                _jumpHeld = false;
            }
        }

        private void Update()
        {
            // Update animation
            _anim.SetFloat("Speed", Mathf.Abs(_moveInput.x));
            _anim.SetBool("IsGrounded", IsGrounded());
            
            // Rotate sprite so the sprite turns the correct way
            if (_moveInput.x > 0.01f)
            {
                _spriteRenderer.flipX = false;
            }
            else if (_moveInput.x < -0.01f)
            {
                _spriteRenderer.flipX = true;
            }
        }

        private void FixedUpdate()
        {
            // Horizontal movement
            Move();
            
            // Check if grounded
            bool grounded = IsGrounded();
            
            // Coyote time counter
            if (grounded)
            {
                _coyoteTimeCounter = coyoteTime;
                _hasDoubleJumped = false;
                _isDoubleJumping = false;
            }
            else
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
            }
            
            // Jump buffer countdown
            if (_jumpBufferCounter > 0)
            {
                _jumpBufferCounter -= Time.fixedDeltaTime;
            }
            
            // Jump logic
            // 1️⃣ First jump (grounded or within coyote time + jump buffer)
            if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
            {
                Jump(jumpForce);
                
                ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.Jump);
                
                _isDoubleJumping = false;
                _jumpStarted = true;

                _jumpQueued = false;
                _jumpBufferCounter = 0f;
            }
            // 2️⃣ Double jump
            else if (canDoubleJump && !_hasDoubleJumped && _jumpQueued)
            {
                Jump(doubleJumpForce);
                
                ManagerRoot.Instance.GameAudioManager.PlaySfx(GameAudioManager.SfxType.Jump);
                
                _hasDoubleJumped = true;
                _isDoubleJumping = true;
                _jumpStarted = true;

                _jumpQueued = false;
            }
            
            // Variable jump height
            if (_jumpStarted && !_jumpHeld && _rb.linearVelocity.y > 0)
            {
                float cutMultiplier = _isDoubleJumping ? doubleJumpCutMultiplier : jumpCutMultiplier;

                _rb.linearVelocity = new Vector2(
                    _rb.linearVelocity.x,
                    _rb.linearVelocity.y * cutMultiplier
                );
                
                _jumpStarted = false;
            }
            
            // Faster fall (optional but feels good)
            if (_rb.linearVelocity.y < 0)
            {
                _rb.gravityScale = fallGravityMultiplier;
            }
            else
            {
                _rb.gravityScale = 1f;
            }
        }

        private void Move()
        {
            _rb.linearVelocity = new Vector2(_moveInput.x * speed, _rb.linearVelocity.y);
                
        }
        
        private void Jump(float force)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, force);
        }

        private bool IsGrounded()
        {
            float rayLength = 0.1f;
            
            Vector2 feetPos = (Vector2)transform.position + Vector2.down * 0.5f; // adjust 0.5f to match half player height
            
            Vector2 leftRay = feetPos + Vector2.left * 0.2f;
            Vector2 rightRay = feetPos + Vector2.right * 0.2f;

            RaycastHit2D hitLeft = Physics2D.Raycast(leftRay, Vector2.down, rayLength, groundLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(rightRay, Vector2.down, rayLength, groundLayer);
            
            // Optional: debug visualization
            Debug.DrawRay(leftRay, Vector2.down * rayLength, Color.red);
            Debug.DrawRay(rightRay, Vector2.down * rayLength, Color.red);

            return hitLeft.collider != null || hitRight.collider != null;
        }
    }
}
