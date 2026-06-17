using UnityEngine;
using UnityEngine.SceneManagement;

namespace JuegoMental
{
    /// <summary>Acciones para botones de UI (menu y game over).</summary>
    public class UIActions : MonoBehaviour
    {
        public void PlayGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Hub");
        }

        public void RestartLevel() => GameManager.RestartLevel();

        public void QuitToMenu() => GameManager.GoToMenu();

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
