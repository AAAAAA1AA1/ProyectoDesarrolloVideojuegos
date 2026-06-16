using UnityEngine;

namespace JuegoMental
{
    public class ChaserEnemy : EnemyBase
    {
        public float speed = 2.5f;
        public float detectRange = 8f;

        [Header("Ataque a distancia")]
        public GameObject projectilePrefab;
        public float shootRange = 9f;
        public float shootCooldown = 2f;
        public float projectileSpeed = 6f;
        public float projectileCortisol = 12f;

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

            // se acerca pero mantiene algo de distancia
            if (dist <= detectRange && dist > 3.5f)
            {
                Vector2 to = (_player.position - transform.position).normalized;
                transform.Translate(new Vector2(to.x, 0f) * speed * Time.deltaTime);
            }

            if (projectilePrefab != null && dist <= shootRange && Time.time >= _nextShot)
            {
                _nextShot = Time.time + shootCooldown;
                var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                var ep = proj.GetComponent<EnemyProjectile>();
                if (ep != null)
                {
                    Vector2 dir = ((Vector2)_player.position - (Vector2)transform.position).normalized;
                    ep.velocity = dir * projectileSpeed;
                    ep.cortisol = projectileCortisol;
                }
            }
        }
    }
}
