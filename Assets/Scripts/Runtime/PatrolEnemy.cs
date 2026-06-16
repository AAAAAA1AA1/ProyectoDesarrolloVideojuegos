using UnityEngine;

namespace JuegoMental
{
    public class PatrolEnemy : EnemyBase
    {
        public float speed = 2f;
        public float range = 3f;

        [Header("Ataque melee")]
        public float meleeRange = 1.7f;
        public float strikeCortisol = 18f;
        public float strikeCooldown = 1.5f;

        Vector2 _start;
        int _dir = 1;
        Transform _player;
        float _nextStrike;

        protected override void Awake()
        {
            base.Awake();
            _start = transform.position;
            var p = GameObject.FindWithTag("Player");
            if (p != null) _player = p.transform;
        }

        void Update()
        {
            transform.Translate(Vector2.right * _dir * speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - _start.x) >= range) _dir = -_dir;

            if (_player != null && Time.time >= _nextStrike &&
                Vector2.Distance(transform.position, _player.position) <= meleeRange)
            {
                _nextStrike = Time.time + strikeCooldown;
                var c = _player.GetComponentInParent<CortisolSystem>();
                if (c != null) c.Add(strikeCortisol);
            }
        }
    }
}
