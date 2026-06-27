using UnityEngine;
using UnityEngine.SceneManagement;

namespace JuegoMental
{
    public class LevelManager : MonoBehaviour
    {
        public float timeLimit = 60f; // Tiempo total en segundos
        public GameObject exitDoor;   // Arrastra aquí tu objeto de la puerta
        private bool _isTimeUp = false;
        public bool isLevelCompleteable = true;

        void Update()
        {
            if (_isTimeUp) return;

            timeLimit -= Time.deltaTime;

            if (timeLimit <= 0)
            {
                timeLimit = 0;
                _isTimeUp = true;
                Debug.Log("¡Tiempo agotado! ¡Corre a la puerta!");
                // Opcional: Activa una animación o mensaje aquí
            }
        }
    }
}
