using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScore : MonoBehaviour, IChannel
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    [SerializeField]
    private GameObject newScore;
    [SerializeField]
    private GameObject registerRanking;
    [SerializeField]
    private GameObject adRowalImage;
    [SerializeField]
    private GameObject registerRanker;

    // 점수 등록을 위한 server 등록
    private EDCServer server;

    private bool isRanker;

    private async void OnEnable()
    {
        var uiManager = Locator<UIManager>.Get();
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.Score, HandleEvent);

        server = Locator<EDCServer>.Get();
        textMeshPro.text = uiManager.GetScore();

        newScore.SetActive(false);
        registerRanking.SetActive(false);
        adRowalImage.SetActive(false);
        isRanker = false;

        int score = 0;

        // tex가 비어 있으면 0점이라는 뜻이다.
        if (string.IsNullOrWhiteSpace(textMeshPro.text))
        {
            textMeshPro.text = "0";
            return;
        }

        if (int.TryParse(textMeshPro.text, out score))
        {
            // 그냥 점수로 랭킹에 들어갈 수 있는지 확인
            if (await server.IsScoreRanker(score))
            {
                // 점수가 들어간다.
                Debug.Log($"New Ranking Score : {score}");
                newScore.SetActive(true);
                registerRanking.SetActive(true);
                //await server.WriteNewScore("Testing", score);
            }
            else
            {
                // 들어오지 않았다.
                Debug.Log("None Puzzle Number");
                newScore.SetActive(false);
            }

            // 광고를 보면 랭킹에 들어갈 수 있는지 확인
            if (await server.IsScoreRanker((int)(score * 1.5f)))
            {
                adRowalImage.SetActive(true);
                isRanker = true;
            }
        }
        else
        {
            Debug.LogError($"Failed to parse score");
        }
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.Score, HandleEvent);
    }

    public void RegisterRanker()
    {
        registerRanker.SetActive(true);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.Score:
                var uiManager = Locator<UIManager>.Get();
                textMeshPro.text = uiManager.GetScore();
                // 광고를 보면 Ranking에 등록할 수 있고, 등록하려면 이름을 입력해야 하기 때문이다.
                if(isRanker)
                    registerRanking.SetActive(true);
                break;
        }
    }

}
