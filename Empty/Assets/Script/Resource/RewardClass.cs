using UnityEngine;

/// <summary>
/// 광고 보상 Reward인 Gold를 담당하는 Reward 함수
/// </summary>
public class GoldReward : IReward
{
    private readonly int gold;
    public GoldReward(int value)
    {
        gold = value;
    }

    public void HandleReward()
    {
        // 캐릭터가 가지고 있는 골드만큼 추가
    }
}

/// <summary>
/// 광고 보상 Reward인 Score를 담당하는 Reward 함수
/// </summary>
public class ScoreReward : IReward
{
    private readonly float multiValue = 1f;

    /// <summary>
    /// Reward 생성자
    /// </summary>
    /// <param name="value">얼만큼 곱할 것인지 정할 변수</param>
    public ScoreReward(float value)
    {
        multiValue = value;
    }

    // 실제 제공할 Reward 보상
    public void HandleReward()
    {
        // 지금 Score를 multiValue만큼 시킨다.
        var uiManager = Locator<UIManager>.Get();
        var score = uiManager.GetScore();

        // 점수를 곱해서 다시 String으로 변경해서 EventManaer로 알린다.
        int scoreValue = int.Parse(score);
        scoreValue = (int)(scoreValue * multiValue);
        uiManager.SetScore(scoreValue.ToString());

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Score);


        Debug.Log($"{multiValue}만큼 증가한다.");
    }
}

