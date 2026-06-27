using UnityEngine;
namespace JuegoMental

{

    public class LevelDoor : MonoBehaviour

    {

        public enum Mode { EnterLevel, ExitToHub }

        public Mode mode = Mode.EnterLevel;

        public int floor = 1;


        bool _playerInside;


        void Start()

        {

            // atenuar puertas ya completadas (no se puede entrar)

            if (mode == Mode.EnterLevel && GameManager.IsCompleted(floor))

            {

                var sr = GetComponent<SpriteRenderer>();

                if (sr != null) sr.color = new Color(0.45f, 0.45f, 0.45f, 1f);

            }

        }


        void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) _playerInside = true; }

        void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) _playerInside = false; }


        void Update()

        {

            if (!_playerInside || !Input.GetKeyDown(KeyCode.E)) return;

            if (mode == Mode.EnterLevel) GameManager.EnterLevel(floor); // ignora si completada/bloqueada

            else GameManager.CompleteLevel(floor);

        }

    }

}