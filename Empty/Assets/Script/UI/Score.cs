using UnityEngine;
using TMPro;

/// <summary>
/// Score를 담당하고 있는 Class
/// </summary>
public class Score : MonoBehaviour, IChannel
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    private EventManager eventManager;
    private UIManager uiManager;

    private void Awake()
    {
        eventManager = Locator<EventManager>.Get();
        uiManager = Locator<UIManager>.Get();
        textMeshPro.text = "0";
    }

    // Event 등록
    private void OnEnable()
    {
        eventManager.Subscription(ChannelInfo.Score, HandleEvent);
        eventManager.Subscription(ChannelInfo.ResetScore, HandleEvent);
    }

    // Event 해지
    private void OnDisable()
    {
        eventManager.Unsubscription(ChannelInfo.Score, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.ResetScore, HandleEvent);
    }

    /// <summary>
    /// Event Manager에 사용할 IChannel Interface
    /// </summary>
    /// <param name="channel">채널 정보</param>
    /// <param name="information">사용할 Object 내용</param>
    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.Score:
                if(information is int value)
                    ChangeScore(value);
                break;
            case ChannelInfo.ResetScore:
                textMeshPro.text = "0";
                break;
        }
    }

    /// <summary>
    /// Score 점수가 바뀐다.
    /// </summary>
    /// <param name="value">바뀔 점수</param>
    private void ChangeScore(int value)
    {
        int currentScore = int.Parse(textMeshPro.text);
        currentScore += value * 100;
        textMeshPro.text = currentScore.ToString();
        uiManager.SetScore(textMeshPro.text);
    }
}
