using UnityEngine;

namespace JuegoMental
{
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Configuración del Ciclo")]
        public float dayDurationInSeconds = 60f; // 5 minutos por día
        public float currentTime = 0f; // Tiempo actual

        [Header("UI / Visual")]
        public UnityEngine.UI.Slider dayProgressSlider; // Arrastra un Slider aquí



        void Update()
        {
            // Aumentamos el tiempo
            currentTime += Time.deltaTime;

            // Calculamos el progreso de 0 a 1 (0% a 100% del día)
            float progress = currentTime / dayDurationInSeconds;

            // Actualizamos la barra de progreso si existe
            if (dayProgressSlider != null)
            {
                dayProgressSlider.value = progress;
            }

            // ¿Se acabó el día?
            if (currentTime >= dayDurationInSeconds)
            {
                EndDay();
            }
        }

        void EndDay()
        {
            Debug.Log("¡El día ha terminado!");
            currentTime = 0; // Reinicia el ciclo
            // Aquí puedes llamar a eventos como: GameController.instance.NextDay();
        }
    }
}