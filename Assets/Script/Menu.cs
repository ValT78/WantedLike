using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject leaderboard;

    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void Leaderboard()
    {
        menu.SetActive(false);
        leaderboard.SetActive(true);
    }

    public void Back()
    {
        menu.SetActive(true);
        leaderboard.SetActive(false);
    }
}
