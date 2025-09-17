using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Ranking�� ����ϴ� Class
/// </summary>
public class Rank : MonoBehaviour
{
    private EDCServer server;

    // Ranking Data
    private List<PlayerScore> rankList;
    
    [SerializeField]
    private GameObject rankBoard;

    [SerializeField]
    private TextMeshProUGUI[] rankers;


    private void Awake()
    {
        server = Locator<EDCServer>.Get();
    }

    // �������� �� server�� �ִ� data�� �д´�.
    private async void OnEnable()
    {
        rankList = await server.ReadRankingListData();
        WriteRankerScore();
    }

    /// <summary>
    /// Exit Button�� ����� �޼���
    /// </summary>
    public void Exit()
    {
        rankBoard.SetActive(false);
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);
    }

    /// <summary>
    /// Ranking System�� ����Ѵ�.
    /// </summary>
    private void WriteRankerScore()
    {
        int length = rankers.Length - rankList.Count;

        // ������ ��ŭ Dummy�� ä��� �Ǵϱ� ������
        for(int i = 0; i < rankList.Count; i++)
        {
            var rankInfo = rankList[i];
            string ranker = ($"{rankInfo.userId}: {rankInfo.score} ");
            rankers[i].text = ranker;
        }

        if (length <= 0)
            return;

        // �� �ʱ⿡ ����ϴ� Dummy Ranker�� ����Ѵ�.
        for(int i = length - 1; i < rankers.Length; i++)
        {
            rankers[i].text = DummyLanker(i);
        }
    }

    /// <summary>
    /// Ranking�� ù ����ؼ� Ranker�� ���ٸ� Dummy Ranker�� �����.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private string DummyLanker(int count)
    {
        return $"EDC {count} : {rankers.Length - count * 100}";
    }
}
