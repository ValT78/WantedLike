using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    private string playerName;
    private int score;
    [SerializeField] private TMPro.TextMeshProUGUI playerNameText;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

    public void SetEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
        playerNameText.text = playerName;
        scoreText.text = score.ToString();
    }
}