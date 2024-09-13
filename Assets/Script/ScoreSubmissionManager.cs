using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreSubmissionManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameInput;

    public void OnSubmitButtonClicked()
    {
        if (playerNameInput.text != "")
        {
            LeaderboardManager.Instance.SetPlayerName(playerNameInput.text);
            LeaderboardManager.Instance.PublishScore(GameManager.Instance.score);
            SceneManager.LoadScene("Menu");
        }
        else
        {
            playerNameInput.text = LeaderboardManager.Instance.RandomPlayerName();

        }
    }

}
