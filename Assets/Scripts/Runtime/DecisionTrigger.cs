using UnityEngine;

namespace JuegoMental
{
    public class DecisionTrigger : MonoBehaviour
    {
        public StressDecisionManager manager;
        public DecisionData data; // Arrastra aquí tu asset de DecisionData

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (manager != null && data != null)
                {
                    manager.StartQuestion(data);
                    Destroy(gameObject);
                }
            }
        }
    }
}