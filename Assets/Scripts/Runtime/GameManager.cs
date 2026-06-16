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

        public static void EnterLevel(int floor)
        {
            if (floor > UnlockedFloor) return;
            SceneManager.LoadScene($"Level_{floor:00}");
        }

        public static void CompleteLevel(int floor)
        {
            if (floor + 1 > UnlockedFloor) UnlockedFloor = floor + 1;
            SceneManager.LoadScene("Hub");
        }

        public static void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public static void GoToHub() => SceneManager.LoadScene("Hub");
    }
}
