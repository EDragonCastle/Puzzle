using UnityEngine;
using TMPro;

/// <summary>
/// ������ Score ������ ��� �ִ� Class
/// </summary>
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

    // Active True ���� ��
    private async void OnEnable()
    {
        // Manager�� �������� Event�� ����Ѵ�.
        var uiManager = Locator<UIManager>.Get();
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.Score, HandleEvent);

        // Server���� Score ������ �����´�.
        server = Locator<EDCServer>.Get();
        textMeshPro.text = uiManager.GetScore();

        // Score ������ ���� Image�� ����� ���� �ʰ� ��� Active False�� �صд�.
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

        // text ������ ������ int ������ �����Ѵ�.
        if (int.TryParse(textMeshPro.text, out score))
        {
            // �ش� ������ ��ŷ�� �� �� �ִ��� Ȯ��
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

    // Event ����
    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.Score, HandleEvent);
    }

    /// <summary>
    /// Button�� ����� �޼���
    /// </summary>
    public void RegisterRanker()
    {
        registerRanker.SetActive(true);
    }


    /// <summary>
    /// Event Manager�� ����� IChannel Interface
    /// </summary>
    /// <param name="channel">ä�� ����</param>
    /// <param name="information">����� Object ����</param>
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
