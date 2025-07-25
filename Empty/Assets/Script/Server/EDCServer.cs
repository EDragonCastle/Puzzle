using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EDCServer
{
    private DatabaseReference dbReference;
    private FirebaseAuth auth;
    public readonly int ranking = 5;

    public async Task InitalizeFirebase()
    {
        var dependancyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependancyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase Realtime Database Initialize");
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
        }
        else
        {
            Debug.LogError($"{dependancyStatus} !!");
        }
    }

    public async Task WriteNewScore(string userId, int score)
    {
        // JSON으로 데이터 생성
        string json = JsonUtility.ToJson(new PlayerScore { userId = userId, score = score });

        try
        {
            await dbReference.Child("scores").Push()
                                             .SetRawJsonValueAsync(json);

            Debug.Log($"Score {score} saved Successfully for user {userId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save score: {e.Message}");
        }
    }

    public async Task ReadRankingData()
    {
        try
        {
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            if (snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    scoreList.Add(playerScore);
                }

                scoreList.Sort((a, b) => b.score.CompareTo(a.score));

                int rank = 1;
                foreach (PlayerScore playerScore in scoreList)
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
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read Score: {e.Message}");
        }
    }

    public async Task<List<PlayerScore>> ReadRankingListData()
    {
        try
        {
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            if (snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    scoreList.Add(playerScore);
                }

                scoreList.Sort((a, b) => b.score.CompareTo(a.score));

                return scoreList;
            }
            else
            {
                Debug.Log("No Ranking Data Found");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read Score: {e.Message}");
            return null;
        }
    }

    public async Task<bool> IsScoreRanker(int currentPlayerScore)
    {

        try
        {
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();

            var topScores = new List<PlayerScore>();
            if (snapshot.Exists)
            {
                foreach (var childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    topScores.Add(playerScore);
                }
            }

            if (topScores.Count < ranking)
            {
                return true;
            }

            topScores.Sort((a, b) => a.score.CompareTo(b.score));
            int lowScore = topScores[0].score;

            if (currentPlayerScore >= lowScore)
                return true;
            else
                return false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to check ranking entry : {e.Message}");
            return false;
        }
    }
}
