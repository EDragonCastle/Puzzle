using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;


public class TestServer : MonoBehaviour
{
    DatabaseReference dbReference;
    FirebaseAuth auth;
    private const int ranking = 5;

    async void Start()
    {
        Debug.Log("Start!");
        await InitalizeFirebase();
    }

    async Task InitalizeFirebase()
    {
        var dependancyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if(dependancyStatus == DependencyStatus.Available)
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase Realtime Database Initialize");

            await WriteNewScore("AAAAF", 5000);

            await ReadRankingData();
        }
        else
        {
            Debug.LogError($"{dependancyStatus} !!");
        }
    }

    async Task WriteNewScore(string userId, int score)
    {
        // JSON으로 데이터 생성
        string json = JsonUtility.ToJson(new PlayerScore { userId = userId, score = score });

        try
        {
            await dbReference.Child("scores").Push()
                                             .SetRawJsonValueAsync(json);

            Debug.Log($"Score {score} saved Successfully for user {userId}");
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Failed to save score: {e.Message}");
        }
    }


    async Task ReadRankingData()
    {
        try
        {
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            if(snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();
                foreach(DataSnapshot childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    scoreList.Add(playerScore);
                }

                scoreList.Sort((a, b) => b.score.CompareTo(a.score));

                int rank = 1;
                foreach(PlayerScore playerScore in scoreList)
                {
                    Debug.Log($"{rank}. User: {playerScore.userId}, Score: {playerScore.score}");
                    rank++;
                }
            }
            else
            {
                Debug.Log("No Ranking Data Found");
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError($"Failed to read Score: {e.Message}");
        }
     
    }
}

