using UnityEngine;

namespace JuegoMental
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        public Sprite idle;
        public Sprite walkA;
        public Sprite walkB;
        public Sprite attack;
        public float stepInterval = 0.14f;
        public float attackFlash = 0.2f;

        SpriteRenderer _sr;
        Rigidbody2D _rb;
        float _t;
        bool _useB;
        float _attackUntil;

        void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();
        }

        public void TriggerAttack() => _attackUntil = Time.time + attackFlash;

        void Update()
        {
            if (Time.time < _attackUntil)
            {
                if (attack != null) _sr.sprite = attack;
                return;
            }

            float speed = _rb != null ? Mathf.Abs(_rb.linearVelocity.x) : 0f;
            if (speed > 0.1f)
            {
                _t += Time.deltaTime;
                if (_t >= stepInterval) { _t = 0f; _useB = !_useB; }
                _sr.sprite = _useB ? walkB : walkA;
            }
            else
            {
                _sr.sprite = idle;
            }
        }
    }
}
