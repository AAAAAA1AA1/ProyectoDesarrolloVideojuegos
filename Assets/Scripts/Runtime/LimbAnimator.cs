using UnityEngine;

namespace JuegoMental
{
    /// <summary>Anima un personaje articulado rotando brazos y piernas (estilo huesos).</summary>
    public class LimbAnimator : MonoBehaviour
    {
        public Transform armFront;
        public Transform armBack;
        public Transform legFront;
        public Transform legBack;
        public GameObject sword;

        public float walkRate = 9f;
        public float swingAmplitude = 38f;
        public float attackDuration = 0.25f;

        Rigidbody2D _rb;
        float _phase;
        float _attackUntil;

        void Awake()
        {
            _rb = GetComponentInParent<Rigidbody2D>();
            if (sword != null) sword.SetActive(false);
        }

        public void TriggerAttack() => _attackUntil = Time.time + attackDuration;

        void Update()
        {
            bool attacking = Time.time < _attackUntil;
            if (sword != null) sword.SetActive(attacking);

            float speed = _rb != null ? Mathf.Abs(_rb.linearVelocity.x) : 0f;

            if (speed > 0.1f)
            {
                _phase += Time.deltaTime * walkRate;
                float s = Mathf.Sin(_phase) * swingAmplitude;
                SetRot(legFront, s);
                SetRot(legBack, -s);
                if (!attacking) SetRot(armFront, -s);
                SetRot(armBack, s);
            }
            else
            {
                EaseToZero(legFront); EaseToZero(legBack); EaseToZero(armBack);
                if (!attacking) EaseToZero(armFront);
            }

            // brazo de ataque hacia adelante
            if (attacking && armFront != null)
                armFront.localRotation = Quaternion.Lerp(armFront.localRotation, Quaternion.Euler(0, 0, -120f), 20f * Time.deltaTime);
        }

        static void SetRot(Transform t, float z)
        {
            if (t != null) t.localRotation = Quaternion.Euler(0f, 0f, z);
        }

        static void EaseToZero(Transform t)
        {
            if (t != null) t.localRotation = Quaternion.Lerp(t.localRotation, Quaternion.identity, 12f * Time.deltaTime);
        }
    }
}
