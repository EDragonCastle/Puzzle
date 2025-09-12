using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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

    // �������� �� server�� �ִ� data�� �о�߰ڳ�.
    private async void OnEnable()
    {
        rankList = await server.ReadRankingListData();
        WriteRankerScore();
    }

    public void Exit()
    {
        rankBoard.SetActive(false);
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);
    }

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

        for(int i = length - 1; i < rankers.Length; i++)
        {
            rankers[i].text = DummyLanker(i);
        }
    }

    private string DummyLanker(int count)
    {
        return $"EDC {count} : {rankers.Length - count * 100}";
    }
}
