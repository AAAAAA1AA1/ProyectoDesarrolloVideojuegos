using UnityEngine;

namespace JuegoMental
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Header("Configuración de Movimiento")]
        public float moveSpeed = 6f;
        public float jumpForce = 12f;
        public int maxJumps = 2;
        public Transform groundCheck;
        public LayerMask groundMask;

        [Header("Configuración de Esqueleto")]
        public Transform characterRoot;
        public bool isFacingRight = true; // Variable pública para que el arma la consulte

        Rigidbody2D _rb;
        Animator _anim;
        int _jumpsLeft;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            float x = Input.GetAxisRaw("Horizontal");
            _rb.linearVelocity = new Vector2(x * moveSpeed, _rb.linearVelocity.y);

            _anim.SetFloat("speed", Mathf.Abs(x));
            _anim.SetBool("isGrounded", IsGrounded());
            _anim.SetFloat("velocityY", _rb.linearVelocity.y);

            if (x != 0)
            {
                isFacingRight = (x > 0);
                float targetYRotation = isFacingRight ? 0f : 180f;

                if (characterRoot.localEulerAngles.y != targetYRotation)
                {
                    Vector3 newRotation = characterRoot.localEulerAngles;
                    newRotation.y = targetYRotation;
                    characterRoot.localEulerAngles = newRotation;
                }
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