using UnityEngine;

namespace JuegoMental
{
    public class PatrolEnemy : EnemyBase
    {
        public float speed = 2f;
        public float range = 5f;
        private Vector2 _startPos;
        private int _dir = 1;

        protected override void Awake()
        {
            base.Awake(); // Llama al Awake de EnemyBase para inicializar la vida
            _startPos = transform.position;
        }

        void Update()
        {
            // Movimiento de patrulla
            transform.Translate(Vector2.right * _dir * speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - _startPos.x) >= range)
            {
                _dir *= -1;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
    }
}