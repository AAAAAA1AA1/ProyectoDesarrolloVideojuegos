using UnityEngine;

namespace JuegoMental
{
    public class PlayerAttack : MonoBehaviour
    {
        public float damage = 1f;
        public float cooldown = 0.35f;
        public float bulletSpeed = 14f;
        public GameObject bulletPrefab;
        public Transform attackOrigin; // boca del cañón

        float _next;
        LimbAnimator _anim;

        void Awake() => _anim = GetComponentInChildren<LimbAnimator>();

        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)) && Time.time >= _next)
            {
                _next = Time.time + cooldown;
                if (_anim != null) _anim.TriggerAttack();
                Shoot();
            }
        }

        void Shoot()
        {
            if (bulletPrefab == null) return;
            float dir = transform.localScale.x < 0f ? -1f : 1f;
            Vector2 origin = attackOrigin != null ? (Vector2)attackOrigin.position : (Vector2)transform.position;
            var b = Instantiate(bulletPrefab, origin, Quaternion.identity);
            var pb = b.GetComponent<PlayerBullet>();
            if (pb != null)
            {
                pb.velocity = new Vector2(dir * bulletSpeed, 0f);
                pb.damage = damage;
            }
            if (dir < 0f) { var s = b.transform.localScale; s.x = -Mathf.Abs(s.x); b.transform.localScale = s; }
        }
    }
}
