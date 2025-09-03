using UnityEngine;

// ��� Reward �߰��� ����ϴ� Class
public class GoldReward : IReward
{
    private readonly int gold;
    public GoldReward(int value)
    {
        gold = value;
    }

    public void HandleReward()
    {
        // ĳ���Ͱ� ������ �ִ� ��常ŭ �߰�
    }
}

// Score �߰��� ����ϴ� Class
public class ScoreReward : IReward
{
    private readonly float multiValue = 1f;
    public ScoreReward(float value)
    {
        multiValue = value;
    }

    public void HandleReward()
    {
        // ���� Score�� multiValue��ŭ ��Ų��.
        var uiManager = Locator<UIManager>.Get();
        var score = uiManager.GetScore();

        int scoreValue = int.Parse(score);
        scoreValue = (int)(scoreValue * multiValue);
        uiManager.SetScore(scoreValue.ToString());

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Score);


        Debug.Log($"{multiValue}��ŭ �����Ѵ�.");
    }
}

