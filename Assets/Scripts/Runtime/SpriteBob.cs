using UnityEngine;

namespace JuegoMental
{
    /// <summary>Da vida a un sprite unico: rebote y leve inclinacion al caminar.</summary>
    public class SpriteBob : MonoBehaviour
    {
        public Transform visual;
        public float bobAmplitude = 0.07f;
        public float bobRate = 12f;
        public float tiltDegrees = 5f;

        Rigidbody2D _rb;
        float _phase;

        void Awake() => _rb = GetComponentInParent<Rigidbody2D>();

        void Update()
        {
            if (visual == null) return;
            float speed = _rb != null ? Mathf.Abs(_rb.linearVelocity.x) : 0f;
            if (speed > 0.1f)
            {
                _phase += Time.deltaTime * bobRate;
                float b = Mathf.Abs(Mathf.Sin(_phase)) * bobAmplitude;
                visual.localPosition = new Vector3(0f, b, 0f);
                visual.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(_phase) * tiltDegrees);
            }
            else
            {
                visual.localPosition = Vector3.Lerp(visual.localPosition, Vector3.zero, 10f * Time.deltaTime);
                visual.localRotation = Quaternion.Lerp(visual.localRotation, Quaternion.identity, 10f * Time.deltaTime);
            }
        }
    }
}
