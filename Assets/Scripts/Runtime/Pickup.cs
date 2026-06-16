using UnityEngine;
using JuegoMental.Core;

namespace JuegoMental
{
    public class Pickup : MonoBehaviour
    {
        public PickupKind kind = PickupKind.Good;
        public float amount = 15f;

        void OnTriggerEnter2D(Collider2D other)
        {
            var c = other.GetComponentInParent<CortisolSystem>();
            if (c == null) return;
            c.Add(new PickupModel(kind, amount).Delta);
            Destroy(gameObject);
        }
    }
}
