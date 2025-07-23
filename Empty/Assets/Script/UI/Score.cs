using UnityEngine;
using TMPro;

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

    private void OnEnable()
    {
        eventManager.Subscription(ChannelInfo.Score, HandleEvent);
        eventManager.Subscription(ChannelInfo.ResetScore, HandleEvent);
    }

    private void OnDisable()
    {
        eventManager.Unsubscription(ChannelInfo.Score, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.ResetScore, HandleEvent);
    }

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

    private void ChangeScore(int value)
    {
        int currentScore = int.Parse(textMeshPro.text);
        currentScore += value * 100;
        textMeshPro.text = currentScore.ToString();
        uiManager.SetScore(textMeshPro.text);
    }
}
