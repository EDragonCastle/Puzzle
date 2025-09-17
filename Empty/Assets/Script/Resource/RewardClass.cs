using UnityEngine;

/// <summary>
/// ���� ���� Reward�� Gold�� ����ϴ� Reward �Լ�
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
        // ĳ���Ͱ� ������ �ִ� ��常ŭ �߰�
    }
}

/// <summary>
/// ���� ���� Reward�� Score�� ����ϴ� Reward �Լ�
/// </summary>
public class ScoreReward : IReward
{
    private readonly float multiValue = 1f;

    /// <summary>
    /// Reward ������
    /// </summary>
    /// <param name="value">��ŭ ���� ������ ���� ����</param>
    public ScoreReward(float value)
    {
        multiValue = value;
    }

    // ���� ������ Reward ����
    public void HandleReward()
    {
        // ���� Score�� multiValue��ŭ ��Ų��.
        var uiManager = Locator<UIManager>.Get();
        var score = uiManager.GetScore();

        // ������ ���ؼ� �ٽ� String���� �����ؼ� EventManaer�� �˸���.
        int scoreValue = int.Parse(score);
        scoreValue = (int)(scoreValue * multiValue);
        uiManager.SetScore(scoreValue.ToString());

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Score);


        Debug.Log($"{multiValue}��ŭ �����Ѵ�.");
    }
}

