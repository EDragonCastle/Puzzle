using UnityEngine;
using TMPro;

public class Score : MonoBehaviour ,IChannel
{
    public TextMeshProUGUI textMeshPro;

    private EventManager eventManager;
    private void Awake()
    {
        eventManager = Locator.GetEventManager();
        textMeshPro.text = "0";
    }

    private void OnEnable()
    {
        eventManager.Subscription(ChannelInfo.Score, HandleEvent);
    }

    private void OnDisable()
    {
        eventManager.Unsubscription(ChannelInfo.Score, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.Score:
                ChangeScore((int)information);
                break;
        }
    }

    private void ChangeScore(int value)
    {
        int currentScore = int.Parse(textMeshPro.text);
        currentScore += value * 100;
        textMeshPro.text = currentScore.ToString();
    }
}
