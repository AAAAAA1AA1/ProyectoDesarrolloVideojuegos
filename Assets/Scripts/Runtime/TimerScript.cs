using UnityEngine;
using UnityEngine.UI;

namespace JuegoMental
{
    public class TimerScript : MonoBehaviour
    {
        public Text timerText;

        void Update()
        {
            // Si LevelManager también está en un namespace, 
            // quizás necesites poner 'using JuegoMental;' arriba.
            var lm = Object.FindFirstObjectByType<LevelManager>();
            if (lm != null)
            {
                timerText.text = "Tiempo: " + Mathf.Ceil(lm.timeLimit).ToString();
            }
        }
    }
}
