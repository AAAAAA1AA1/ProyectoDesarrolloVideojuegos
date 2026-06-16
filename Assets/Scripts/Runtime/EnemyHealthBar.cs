using UnityEngine;

namespace JuegoMental
{
    public class EnemyHealthBar : MonoBehaviour
    {
        public EnemyBase enemy;
        public Transform fill;      // escala X 0..1
        public Vector3 offset = new Vector3(0f, 0.8f, 0f);

        Vector3 _fullScale;

        void Start()
        {
            if (fill != null) _fullScale = fill.localScale;
            if (enemy != null) enemy.OnHealthChanged += SetFraction;
        }

        void OnDestroy()
        {
            if (enemy != null) enemy.OnHealthChanged -= SetFraction;
        }

        void LateUpdate()
        {
            if (enemy != null) transform.position = enemy.transform.position + offset;
        }

        void SetFraction(float f)
        {
            if (fill != null) fill.localScale = new Vector3(_fullScale.x * f, _fullScale.y, _fullScale.z);
        }
    }
}
