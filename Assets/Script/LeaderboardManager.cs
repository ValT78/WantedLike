using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Core;
using System.Threading.Tasks;
using LootLocker.Requests;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    [SerializeField] private GameObject entryPrefab;
    private readonly string leaderboardKey = "WantedBDAKey";
    public float timeBetweenScoreUpdate = 10f;
    [HideInInspector] public string playerName;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Login();
    }

    public void Login()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (!response.success)
                {
                    Debug.Log("error starting LootLocker session" + response.errorData);
                    return;
                }
                playerName = response.player_name;
     

                Debug.Log("successfully started LootLocker session");
            });

    }

    public void PublishScore(int score)
    {

        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, (response) =>
        {
            if (!response.success)
            {
                Debug.Log("Could not submit score!");
                //Debug.Log(response.errorData.ToString());
                return;
            }
            Debug.Log("Successfully submitted score!");

        });

    }


    public void SetPlayerName(string name)
    {

        LootLockerSDKManager.SetPlayerName(name, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Succesfully set player name");
                playerName = name;
            }
            else
            {
                Debug.Log("Could not set player name");
                //Debug.Log(response.errorData.ToString());
                return;
            }
        });
        Debug.Log("Changing name to " + name);
    }

    public void DisplayLeaderboard(Transform leaderboardTable)
    {
        // Effacer les anciennes entrées
        foreach (Transform child in leaderboardTable)
        {
            Destroy(child.gameObject);
        }
        int count = 150;

        // Récupérer les scores depuis le serveur
        LootLockerSDKManager.GetScoreList(leaderboardKey, count, 0, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] members = response.items;
                var accounts = new Dictionary<string, int>();


                for (int i = 0; i < members.Length; i++)
                {
                    string name = members[i].player.name != "" ? members[i].player.name.ToLower() : members[i].player.id.ToString().ToLower(); 
                    int score = members[i].score;
                    
                    if(accounts.ContainsKey(name))
                    {
                        if (accounts[name] > score)
                        {
                            continue;
                        }
                    }
                    accounts.Add(name, score);
                }
                foreach (var account in accounts)
                {
                    Instantiate(entryPrefab, leaderboardTable).GetComponent<LeaderboardEntryUI>().SetEntry(account.Key, account.Value);
                }
            }
            else
            {
                Debug.Log("Failed" + response.errorData);
            }
        });

        
    }

}
