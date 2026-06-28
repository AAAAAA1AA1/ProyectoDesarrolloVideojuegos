using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("Configuración de Game Over")]
        public CortisolSystem cortisol;
        public GameObject panel;
        public Text statusText;

        [Header("Elementos de UI a limpiar")]
        public GameObject[] objetosParaOcultar;

        void Awake()
        {
            // 1. FORZAMOS EL TIEMPO A 1 AL CARGAR LA ESCENA
            Time.timeScale = 1f;

            // 2. Aseguramos que el panel esté oculto apenas inicia
            if (panel != null) panel.SetActive(false);
        }

        void OnEnable()
        {
            if (cortisol == null) cortisol = FindObjectOfType<CortisolSystem>();

            if (cortisol != null)
            {
                cortisol.OnLost += ShowByStress;
            }
        }

        void OnDisable()
        {
            if (cortisol != null) cortisol.OnLost -= ShowByStress;
        }

        public void ShowByStress() => Show("¡Estrés al máximo!");
        public void ShowByTime() => Show("¡Se acabó el tiempo!");

        void Show(string message)
        {
            LevelManager lm = FindObjectOfType<LevelManager>();
            if (lm != null)
            {
                lm.EsconderMensajes();
                if (lm.mensajeInicio != null) lm.mensajeInicio.SetActive(false);
            }

            if (objetosParaOcultar != null)
            {
                foreach (GameObject obj in objetosParaOcultar)
                {
                    if (obj != null) obj.SetActive(false);
                }
            }

            if (statusText != null) statusText.text = message;

            if (panel != null)
            {
                panel.SetActive(true);
            }

            // 3. Solo pausamos si el panel realmente se activó
            Time.timeScale = 0f;
        }
    }
}