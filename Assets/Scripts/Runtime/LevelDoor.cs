using UnityEngine;

namespace JuegoMental
{
    public class LevelDoor : MonoBehaviour
    {
        public enum Mode { EnterLevel, ExitToHub }
        public Mode mode = Mode.EnterLevel;
        public int floor = 1;

        [Header("Configuración Visual")]
        public SpriteRenderer spriteRenderer;
        public GameObject textoSuperado; // Arrastra el texto aquí

        bool _playerInside;

        void Start()
        {
            if (mode == Mode.ExitToHub) return;

            // Lógica de estado
            bool completado = GameManager.IsCompleted(floor);
            bool bloqueado = floor > GameManager.UnlockedFloor;

            if (completado)
            {
                if (spriteRenderer != null) spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
                if (textoSuperado != null) textoSuperado.SetActive(true);
            }
            else if (bloqueado)
            {
                if (spriteRenderer != null) spriteRenderer.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                GetComponent<Collider2D>().enabled = false; // Bloquea la entrada
            }
            else
            {
                if (textoSuperado != null) textoSuperado.SetActive(false);
            }
        }

        void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) _playerInside = true; }
        void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) _playerInside = false; }

        void Update()
        {
            if (_playerInside && Input.GetKeyDown(KeyCode.E))
            {
                if (mode == Mode.EnterLevel)
                {
                    GameManager.EnterLevel(floor);
                }
                else
                {
                    GameManager.CompleteLevel(floor);
                }
            }
        }
    }
}