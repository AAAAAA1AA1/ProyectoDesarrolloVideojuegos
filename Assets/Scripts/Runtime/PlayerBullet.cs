using UnityEngine;

namespace JuegoMental
{
    public class PlayerBullet : MonoBehaviour
    {
        public Vector2 velocity;
        public float damage = 1f;
        public float life = 2.5f;

        void Start() => Destroy(gameObject, life);

        void Update() => transform.Translate(velocity * Time.deltaTime, Space.World);

        void OnTriggerEnter2D(Collider2D other)
        {
            var enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
