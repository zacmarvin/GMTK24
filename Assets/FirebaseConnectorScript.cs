using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FirebaseConnectorScript : MonoBehaviour
{
    public string firebaseUrl = "https://scales-a4d10-default-rtdb.firebaseio.com/leaderboard.json";
    // Start is called before the first frame update
    void Start()
    {
        AddScore("Player 1", 100);

    }
    public void AddScore(string playerName, int score)
    {
        StartCoroutine(PostScore(playerName, score));
    }

    IEnumerator PostScore(string playerName, int score)
    {
        // Create a leaderboard entry
        LeaderboardEntry entry = new LeaderboardEntry { playerName = playerName, score = score };
        string json = JsonUtility.ToJson(entry);

        // Create a POST request to add the entry to the leaderboard
        UnityWebRequest request = new UnityWebRequest(firebaseUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score added successfully!");
        }
        else
        {
            Debug.LogError("Failed to add score: " + request.error);
        }
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
    }
}
