using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreSubmissionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI holderText;
    [SerializeField] private TextMeshProUGUI textInputField;

    void Start()
    {
        playerNameInput.text = LeaderboardManager.Instance.playerName;
    }

    public void OnSubmitButtonClicked()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = "EmptyPseudo";
        }
        else
        {
            SoundManager.Instance.PlaySFX(SoundManager.Instance.chooseMenu);
            LeaderboardManager.Instance.SetPlayerName(playerNameInput.text);
            LeaderboardManager.Instance.PublishScore(GameManager.Instance.score);
            SceneManager.LoadScene("Menu");
        }
     
    }

    public void OnSelectField()
    {
        holderText.text = "";
    }

    public void OnDeselectField()
    {
        if (playerNameInput.text == "")
        {
            holderText.text = "Type Pseudo...";
        }
    }

}
