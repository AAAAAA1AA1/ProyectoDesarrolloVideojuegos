using UnityEngine;

namespace JuegoMental
{
    public class LevelDoor : MonoBehaviour
    {
        public enum Mode { EnterLevel, ExitToHub }
        public Mode mode = Mode.EnterLevel;
        public int floor = 1;

        bool _playerInside;

        void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) _playerInside = true; }
        void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) _playerInside = false; }

        void Update()
        {
            if (!_playerInside) return;
            if (mode == Mode.EnterLevel && Input.GetKeyDown(KeyCode.E))
                GameManager.EnterLevel(floor);
            else if (mode == Mode.ExitToHub && Input.GetKeyDown(KeyCode.E))
                GameManager.CompleteLevel(floor);
        }

        public bool Unlocked => floor <= GameManager.UnlockedFloor;
    }
}
