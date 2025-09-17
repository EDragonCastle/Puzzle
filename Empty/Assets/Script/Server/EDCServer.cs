using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Firebase Server와 연동하는 곳이다.
/// </summary>
public class EDCServer
{
    private DatabaseReference dbReference;
    private FirebaseAuth auth;

    // 몇 번째까지 ranking에 들것인지 정하는 변수
    public readonly int ranking = 5;

    /// <summary>
    /// 초기 Firebase를 연동할 수 있도록 하는 함수다.
    /// </summary>
    /// <returns></returns>
    public async UniTask InitalizeFirebase()
    {
        // StreamingAssets/google-service 파일을 확인한다.
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

    /// <summary>
    /// Score를 Server에 저장할 수 있도록 하는 함수다.
    /// </summary>
    /// <param name="userId">랭킹에 올릴 닉네임(현재 실제 Server ID는 아니다)</param>
    /// <param name="score">점수</param>
    /// <returns></returns>
    public async UniTask WriteNewScore(string userId, int score)
    {
        // JSON으로 데이터 생성
        string json = JsonUtility.ToJson(new PlayerScore { userId = userId, score = score });

        try
        {
            // Firebase Data에 Score를 넣는다.
            await dbReference.Child("scores").Push()
                                             .SetRawJsonValueAsync(json);

            Debug.Log($"Score {score} saved Successfully for user {userId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save score: {e.Message}");
        }
    }

    /// <summary>
    /// Firebase Server에 현재 RankingData를 넣는다.
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadRankingData()
    {
        try
        {
            // DB에서 Score 부분을 확인한다.
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            // DB가 존재하는지 확인한다.
            if (snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();
                // DB를 순회하면서 Player 정보를 담는다.
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    scoreList.Add(playerScore);
                }

                // 내림차순으로 정렬한다.
                scoreList.Sort((a, b) => b.score.CompareTo(a.score));

                int rank = 1;
                // Debug창에 알린다.
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

    /// <summary>
    /// Firebase의 Ranking Data를 정보를 가져온다.
    /// </summary>
    /// <returns></returns>
    public async UniTask<List<PlayerScore>> ReadRankingListData()
    {
        try
        {
            // DB에서 Score 부분을 가져온다.
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            // snapshot이 존재하는지 확인한다.
            if (snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();

                // 순회한뒤 내림차순으로 정렬하고 출력한다.
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

    /// <summary>
    /// 해당 점수가 Ranking에 드는지 안 드는지 확인한다.
    /// </summary>
    /// <param name="currentPlayerScore">현재 점수</param>
    /// <returns></returns>
    public async UniTask<bool> IsScoreRanker(int currentPlayerScore)
    {

        try
        {
            // DB에서 Score 부분을 확인한다.
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();

            var topScores = new List<PlayerScore>();
            if (snapshot.Exists)
            {
                // 순
                foreach (var childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    topScores.Add(playerScore);
                }
            }

            // ranking 보다 적다면 return 한다.
            if (topScores.Count < ranking)
            {
                return true;
            }

            // 오름차순으로 확인 후 0번째 값을 비교한다.
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
