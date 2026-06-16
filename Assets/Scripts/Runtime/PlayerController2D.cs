using UnityEngine;

namespace JuegoMental
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 9f;
        public Transform groundCheck;
        public LayerMask groundMask;

        Rigidbody2D _rb;
        SpriteRenderer _sr;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            _rb.linearVelocity = new Vector2(x * moveSpeed, _rb.linearVelocity.y);
            if (x != 0 && _sr != null) _sr.flipX = x < 0;

            if (Input.GetButtonDown("Jump") && IsGrounded())
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
        }

        bool IsGrounded()
        {
            if (groundCheck == null) return true;
            return Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundMask);
        }
    }
}
