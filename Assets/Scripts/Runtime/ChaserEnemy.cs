using UnityEngine;

namespace JuegoMental
{
    public class ChaserEnemy : EnemyBase
    {
        public float speed = 2.5f;
        public float detectRange = 6f;
        Transform _player;

        protected override void Awake()
        {
            base.Awake();
            var p = GameObject.FindWithTag("Player");
            if (p != null) _player = p.transform;
        }

        void Update()
        {
            if (_player == null) return;
            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist <= detectRange)
            {
                Vector2 to = (_player.position - transform.position).normalized;
                transform.Translate(new Vector2(to.x, 0f) * speed * Time.deltaTime);
            }
        }
    }
}
