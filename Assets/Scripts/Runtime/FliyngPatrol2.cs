using UnityEngine;

namespace JuegoMental
{
    public class FliyngPatrol2 : EnemyBase
    {
        public float patrolDistance = 5f; // Distancia desde donde lo pongas
        public float speed = 2f;

        private Vector3 _startPos;
        private Vector3 _targetPos;
        private bool _movingToEnd = true;

        protected override void Awake()
        {
            base.Awake();
            _startPos = transform.position;
            _targetPos = _startPos + new Vector3(patrolDistance, 0, 0);
        }

        void Update()
        {
            Vector3 target = _movingToEnd ? _targetPos : _startPos;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target) < 0.1f)
            {
                _movingToEnd = !_movingToEnd;
                // Voltear sprite
                transform.localScale = new Vector3(_movingToEnd ? 1 : -1, 1, 1);
            }
        }

        void OnTriggerStay2D(Collider2D other) => TryContact(other);
    }
}