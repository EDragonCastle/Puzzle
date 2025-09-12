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

    // ���� ����� ���� server ���
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

        // tex�� ��� ������ 0���̶�� ���̴�.
        if (string.IsNullOrWhiteSpace(textMeshPro.text))
        {
            textMeshPro.text = "0";
            return;
        }

        if (int.TryParse(textMeshPro.text, out score))
        {
            // �׳� ������ ��ŷ�� �� �� �ִ��� Ȯ��
            if (await server.IsScoreRanker(score))
            {
                // ������ ����.
                Debug.Log($"New Ranking Score : {score}");
                newScore.SetActive(true);
                registerRanking.SetActive(true);
                //await server.WriteNewScore("Testing", score);
            }
            else
            {
                // ������ �ʾҴ�.
                Debug.Log("None Puzzle Number");
                newScore.SetActive(false);
            }

            // ���� ���� ��ŷ�� �� �� �ִ��� Ȯ��
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
                // ���� ���� Ranking�� ����� �� �ְ�, ����Ϸ��� �̸��� �Է��ؾ� �ϱ� �����̴�.
                if(isRanker)
                    registerRanking.SetActive(true);
                break;
        }
    }

}
