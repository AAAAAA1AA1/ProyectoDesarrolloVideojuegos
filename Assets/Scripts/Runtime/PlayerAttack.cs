using UnityEngine;

namespace JuegoMental
{
    public class PlayerAttack : MonoBehaviour
    {
        public float damage = 1f;
        public float range = 0.8f;
        public float cooldown = 0.4f;
        public LayerMask enemyMask;
        public Transform attackOrigin; // punto frente al jugador

        float _next;
        LimbAnimator _anim;

        void Awake()
        {
            _anim = GetComponentInChildren<LimbAnimator>();
        }

        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= _next)
            {
                _next = Time.time + cooldown;
                if (_anim != null) _anim.TriggerAttack();
                DoAttack();
            }
        }

        void DoAttack()
        {
            Vector2 dir = transform.localScale.x < 0f ? Vector2.left : Vector2.right;
            Vector2 origin = attackOrigin != null
                ? (Vector2)attackOrigin.position
                : (Vector2)transform.position + dir * 0.5f;
            var hits = Physics2D.OverlapCircleAll(origin, range, enemyMask);
            foreach (var h in hits)
            {
                var enemy = h.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(damage);
            }
        }
    }
}
