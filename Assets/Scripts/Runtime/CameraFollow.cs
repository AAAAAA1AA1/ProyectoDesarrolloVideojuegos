using UnityEngine;

namespace JuegoMental
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public float smooth = 5f;
        public Vector2 offset = new Vector2(0f, 1f);

        void LateUpdate()
        {
            if (target == null) return;
            Vector3 goal = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, goal, smooth * Time.deltaTime);
        }
    }
}
