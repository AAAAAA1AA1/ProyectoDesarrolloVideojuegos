using UnityEngine;

namespace JuegoMental
{
    public class EnemyProjectile : MonoBehaviour
    {
        public Vector2 velocity;
        public float cortisol = 12f;
        public float life = 4f;

        void Start() => Destroy(gameObject, life);

        void Update() => transform.Translate(velocity * Time.deltaTime, Space.World);

        void OnTriggerEnter2D(Collider2D other)
        {
            var c = other.GetComponentInParent<CortisolSystem>();
            if (c != null)
            {
                c.Add(cortisol);
                Destroy(gameObject);
            }
        }
    }
}
