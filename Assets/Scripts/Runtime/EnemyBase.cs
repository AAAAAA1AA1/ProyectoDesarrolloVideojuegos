using UnityEngine;
using JuegoMental.Core;

namespace JuegoMental
{
    public abstract class EnemyBase : MonoBehaviour
    {
        public float maxHp = 3f;
        public float contactCortisol = 15f;
        public float contactCooldown = 1f;

        protected EnemyHealth Health;
        float _nextContact;

        public System.Action<float> OnHealthChanged;

        protected virtual void Awake() => Health = new EnemyHealth(maxHp);

        public void TakeDamage(float amount)
        {
            Health.Damage(amount);
            OnHealthChanged?.Invoke(Health.Fraction);
            if (Health.IsDead) Destroy(gameObject);
        }

        // Se mantienen como private ya que Unity llama a estos mensajes internamente
        void OnCollisionStay2D(Collision2D col) => TryContact(col.collider);
        void OnTriggerStay2D(Collider2D other) => TryContact(other);

        // CORRECCIÓN: Cambiado de 'void' a 'protected void'
        protected void TryContact(Collider2D other)
        {
            if (Time.time < _nextContact) return;
            var c = other.GetComponentInParent<CortisolSystem>();
            if (c != null)
            {
                _nextContact = Time.time + contactCooldown;
                c.Add(contactCortisol);
            }
        }
    }
}