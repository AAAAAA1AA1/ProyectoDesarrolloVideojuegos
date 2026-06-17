using UnityEngine;
using UnityEngine.SceneManagement;

namespace JuegoMental
{
    public class GameManager : MonoBehaviour
    {
        const string UnlockedKey = "unlocked_floor";

        public static int UnlockedFloor
        {
            get => Mathf.Max(1, PlayerPrefs.GetInt(UnlockedKey, 1));
            set { PlayerPrefs.SetInt(UnlockedKey, value); PlayerPrefs.Save(); }
        }

        public static bool IsCompleted(int floor) => PlayerPrefs.GetInt("completed_" + floor, 0) == 1;

        public static void MarkCompleted(int floor)
        {
            PlayerPrefs.SetInt("completed_" + floor, 1);
            PlayerPrefs.Save();
        }

        public static void EnterLevel(int floor)
        {
            if (floor > UnlockedFloor || IsCompleted(floor)) return;
            Time.timeScale = 1f;
            SceneManager.LoadScene($"Level_{floor:00}");
        }

        public static void CompleteLevel(int floor)
        {
            MarkCompleted(floor);
            if (floor + 1 > UnlockedFloor) UnlockedFloor = floor + 1;
            Time.timeScale = 1f;
            SceneManager.LoadScene("Hub");
        }

        public static void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void GoToHub()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Hub");
        }

        public static void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
