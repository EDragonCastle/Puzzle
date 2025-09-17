using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// Firebase Server�� �����ϴ� ���̴�.
/// </summary>
public class EDCServer
{
    private DatabaseReference dbReference;
    private FirebaseAuth auth;

    // �� ��°���� ranking�� ������� ���ϴ� ����
    public readonly int ranking = 5;

    /// <summary>
    /// �ʱ� Firebase�� ������ �� �ֵ��� �ϴ� �Լ���.
    /// </summary>
    /// <returns></returns>
    public async UniTask InitalizeFirebase()
    {
        // StreamingAssets/google-service ������ Ȯ���Ѵ�.
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
    /// Score�� Server�� ������ �� �ֵ��� �ϴ� �Լ���.
    /// </summary>
    /// <param name="userId">��ŷ�� �ø� �г���(���� ���� Server ID�� �ƴϴ�)</param>
    /// <param name="score">����</param>
    /// <returns></returns>
    public async UniTask WriteNewScore(string userId, int score)
    {
        // JSON���� ������ ����
        string json = JsonUtility.ToJson(new PlayerScore { userId = userId, score = score });

        try
        {
            // Firebase Data�� Score�� �ִ´�.
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
    /// Firebase Server�� ���� RankingData�� �ִ´�.
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadRankingData()
    {
        try
        {
            // DB���� Score �κ��� Ȯ���Ѵ�.
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            // DB�� �����ϴ��� Ȯ���Ѵ�.
            if (snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();
                // DB�� ��ȸ�ϸ鼭 Player ������ ��´�.
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    scoreList.Add(playerScore);
                }

                // ������������ �����Ѵ�.
                scoreList.Sort((a, b) => b.score.CompareTo(a.score));

                int rank = 1;
                // Debugâ�� �˸���.
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
    /// Firebase�� Ranking Data�� ������ �����´�.
    /// </summary>
    /// <returns></returns>
    public async UniTask<List<PlayerScore>> ReadRankingListData()
    {
        try
        {
            // DB���� Score �κ��� �����´�.
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();
            // snapshot�� �����ϴ��� Ȯ���Ѵ�.
            if (snapshot.Exists)
            {
                Debug.Log("Current Ranking Data");
                var scoreList = new List<PlayerScore>();

                // ��ȸ�ѵ� ������������ �����ϰ� ����Ѵ�.
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
    /// �ش� ������ Ranking�� ����� �� ����� Ȯ���Ѵ�.
    /// </summary>
    /// <param name="currentPlayerScore">���� ����</param>
    /// <returns></returns>
    public async UniTask<bool> IsScoreRanker(int currentPlayerScore)
    {

        try
        {
            // DB���� Score �κ��� Ȯ���Ѵ�.
            DataSnapshot snapshot = await dbReference.Child("scores")
                                                     .OrderByChild("score")
                                                     .LimitToLast(ranking)
                                                     .GetValueAsync();

            var topScores = new List<PlayerScore>();
            if (snapshot.Exists)
            {
                // ��
                foreach (var childSnapshot in snapshot.Children)
                {
                    PlayerScore playerScore = JsonUtility.FromJson<PlayerScore>(childSnapshot.GetRawJsonValue());
                    topScores.Add(playerScore);
                }
            }

            // ranking ���� ���ٸ� return �Ѵ�.
            if (topScores.Count < ranking)
            {
                return true;
            }

            // ������������ Ȯ�� �� 0��° ���� ���Ѵ�.
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
