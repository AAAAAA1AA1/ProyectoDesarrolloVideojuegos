using UnityEngine;

namespace JuegoMental
{
    public class FlyingPatrolEnemy : EnemyBase
    {
        public Transform pointA;
        public Transform pointB;
        public float speed = 2f;
        private Transform _target;

        protected override void Awake()
        {
            base.Awake(); // Inicializa salud
            _target = pointB;
        }

        void Update()
        {
            // Movimiento
            transform.position = Vector3.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);

            // Cambiar dirección
            if (Vector3.Distance(transform.position, _target.position) < 0.1f)
                _target = (_target == pointA) ? pointB : pointA;

            // Voltear sprite (si el pájaro mira a la derecha o izquierda)
            float direction = _target.position.x - transform.position.x;
            if (direction != 0)
                transform.localScale = new Vector3(Mathf.Sign(direction), 1, 1);
        }

        // Lógica de daño heredada de EnemyBase
        void OnTriggerStay2D(Collider2D other) => TryContact(other);
    }
}