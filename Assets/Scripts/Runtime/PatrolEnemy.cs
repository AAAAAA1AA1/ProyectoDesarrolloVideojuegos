using UnityEngine;

namespace JuegoMental
{
    public class PatrolEnemy : EnemyBase
    {
        public float speed = 2f;
        public float range = 3f;

        Vector2 _start;
        int _dir = 1;

        protected override void Awake()
        {
            base.Awake();
            _start = transform.position;
        }

        void Update()
        {
            transform.Translate(Vector2.right * _dir * speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - _start.x) >= range) _dir = -_dir;
        }
    }
}
