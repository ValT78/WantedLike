using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private Transform leaderboardTable;

    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        SoundManager.Instance.PlaySFX(SoundManager.Instance.chooseMenu);
    }

    public void Leaderboard()
    {
        menu.SetActive(false);
        leaderboard.SetActive(true);
        LeaderboardManager.Instance.DisplayLeaderboard(leaderboardTable);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.chooseMenu);
    }

    public void Back()
    {
        menu.SetActive(true);
        leaderboard.SetActive(false);
        SoundManager.Instance.PlaySFX(SoundManager.Instance.chooseMenu);
    }
}
