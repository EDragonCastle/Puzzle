using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Ranking을 담당하는 Class
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

    // 시작했을 때 server에 있는 data를 읽는다.
    private async void OnEnable()
    {
        rankList = await server.ReadRankingListData();
        WriteRankerScore();
    }

    /// <summary>
    /// Exit Button에 사용할 메서드
    /// </summary>
    public void Exit()
    {
        rankBoard.SetActive(false);
        var uiManager = Locator<UIManager>.Get();
        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        title.SetActive(true);
    }

    /// <summary>
    /// Ranking System에 등록한다.
    /// </summary>
    private void WriteRankerScore()
    {
        int length = rankers.Length - rankList.Count;

        // 부족한 만큼 Dummy로 채우면 되니까 지금은
        for(int i = 0; i < rankList.Count; i++)
        {
            var rankInfo = rankList[i];
            string ranker = ($"{rankInfo.userId}: {rankInfo.score} ");
            rankers[i].text = ranker;
        }

        if (length <= 0)
            return;

        // 맨 초기에 사용하는 Dummy Ranker를 등록한다.
        for(int i = length - 1; i < rankers.Length; i++)
        {
            rankers[i].text = DummyLanker(i);
        }
    }

    /// <summary>
    /// Ranking에 첫 등록해서 Ranker가 없다면 Dummy Ranker를 만든다.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private string DummyLanker(int count)
    {
        return $"EDC {count} : {rankers.Length - count * 100}";
    }
}
