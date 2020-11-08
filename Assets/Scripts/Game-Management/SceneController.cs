using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void LoadMainMenu() => SceneManager.LoadScene("Menu");
    public static void LoadGame() { SceneManager.LoadScene("Game"); }
}
