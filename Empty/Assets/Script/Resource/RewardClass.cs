using UnityEngine;

// 골드 Reward 추가를 담당하는 Class
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

// Score 추가를 담당하는 Class
public class ScoreReward : IReward
{
    private readonly float multiValue = 1f;
    public ScoreReward(float value)
    {
        multiValue = value;
    }

    public void HandleReward()
    {
        // 지금 Score를 multiValue만큼 시킨다.
        var uiManager = Locator<UIManager>.Get();
        var score = uiManager.GetScore();

        int scoreValue = int.Parse(score);
        scoreValue = (int)(scoreValue * multiValue);
        uiManager.SetScore(scoreValue.ToString());

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Score);


        Debug.Log($"{multiValue}만큼 증가한다.");
    }
}

