using UnityEngine;

namespace JuegoMental
{
    public class ChaserEnemy : EnemyBase
    {
        public float speed = 8f; // Aumentada
        public float detectRange = 10f;
        public float stopDistance = 3f; // Distancia mínima al jugador

        [Header("Ataque a distancia")]
        public GameObject projectilePrefab;
        public float shootRange = 9f;
        public float shootCooldown = 2f;
        public float projectileSpeed = 6f;

        Transform _player;
        float _nextShot;

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
                // Moverse hacia el jugador si está fuera del radio de parada
                if (dist > stopDistance)
                {
                    MoveTowardsPlayer();
                }

                // Disparo
                if (dist <= shootRange && Time.time >= _nextShot)
                {
                    Shoot();
                }
            }
        }

        void MoveTowardsPlayer()
        {
            Vector2 direction = (_player.position - transform.position).normalized;

            // Detección de obstáculos (Esquiva simple)
            // Lanza un rayo frente al enemigo para ver si hay una pared
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, LayerMask.GetMask("Default"));

            if (hit.collider != null)
            {
                // Si hay pared, intenta moverse verticalmente para "rodear"
                direction = new Vector2(0, direction.y > 0 ? 1 : -1);
            }

            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + direction, speed * Time.deltaTime);
        }

        void Shoot()
        {
            _nextShot = Time.time + shootCooldown;
            var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            var ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                ep.velocity = (_player.position - transform.position).normalized * projectileSpeed;
                ep.cortisol = contactCortisol;
            }
        }
    }
}