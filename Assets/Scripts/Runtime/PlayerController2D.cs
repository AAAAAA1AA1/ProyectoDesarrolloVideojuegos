using UnityEngine;

namespace JuegoMental
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        public float moveSpeed = 6f;
        public float jumpForce = 12f;
        public int maxJumps = 2;          // 2 = doble salto
        public Transform groundCheck;
        public LayerMask groundMask;

        Rigidbody2D _rb;
        int _jumpsLeft;

        void Awake() => _rb = GetComponent<Rigidbody2D>();

        void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            _rb.linearVelocity = new Vector2(x * moveSpeed, _rb.linearVelocity.y);

            if (x != 0)
            {
                var s = transform.localScale;
                s.x = Mathf.Abs(s.x) * (x < 0 ? -1f : 1f);
                transform.localScale = s;
            }

            if (IsGrounded() && _rb.linearVelocity.y <= 0.01f)
                _jumpsLeft = maxJumps;

            if (Input.GetButtonDown("Jump") && _jumpsLeft > 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
                _jumpsLeft--;
            }
        }

        bool IsGrounded()
        {
            if (groundCheck == null) return true;
            return Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundMask);
        }
    }
}
