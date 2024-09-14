using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreSubmissionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI holderText;
    [SerializeField] private TextMeshProUGUI textInputField;
    private TouchScreenKeyboard keyboard;


    void Start()
    {
        playerNameInput.text = LeaderboardManager.Instance.playerName;
    }

    void Update()
    {
        if (keyboard != null && keyboard.active)
        {
            playerNameInput.text = keyboard.text;
        }
    }

    public void OnSubmitButtonClicked()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            playerNameInput.text = GenerateRandomPseudo();
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
        holderText.text = LeaderboardManager.Instance.playerName;
        playerNameInput.Select(); // Sélectionne le champ de texte
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void OnDeselectField()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            holderText.text = "Type Pseudo...";
        }
    }

    public string GenerateRandomPseudo()
    {
        string[] adjectives = { "Happy", "Crazy", "Funny", "Silly", "Witty", "Brave", "Clever", "Jolly", "Zany", "Quirky", "Mighty", "Swift", "Bold", "Cheerful", "Daring", "Eager", "Fierce", "Gentle", "Heroic" };
        string[] nouns = { "Penguin", "Ninja", "Pirate", "Unicorn", "Robot", "Dragon", "Wizard", "Alien", "Zombie", "Knight", "Phoenix", "Tiger", "Lion", "Bear", "Eagle", "Shark", "Wolf", "Panther", "Falcon", "Hawk" };

        System.Random random = new();
        string adjective = adjectives[random.Next(adjectives.Length)];
        string noun = nouns[random.Next(nouns.Length)];

        string pseudo = adjective + noun;

        // Ensure the pseudo is 15 characters or less
        if (pseudo.Length > 15)
        {
            pseudo = pseudo.Substring(0, 15);
        }

        return pseudo;
    }
}
