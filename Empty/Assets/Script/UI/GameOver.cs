using UnityEngine;

/// <summary>
/// GameOver를 담당하는 Class
/// </summary>
public class GameOver : MonoBehaviour
{
    private GameObject title;
    private GameObject gameOver;
    private GameObject board;
    private GameObject ranking;

    [SerializeField]
    private GameObject adButton;

    // 시작하면 Manager에서 관련 Object를 가져온다.
    private void Start()
    {
        var uiManager = Locator<UIManager>.Get();
        title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
        board = uiManager.GetUIPrefabObject(UIPrefab.Board);
        ranking = uiManager.GetUIPrefabObject(UIPrefab.Rank);
    }

    private void OnEnable()
    {
        adButton.SetActive(true);
    }

    /// <summary>
    /// Game Start Button에 사용할 메서드
    /// </summary>
    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        ranking.SetActive(false);
        board.SetActive(true);
    }

    /// <summary>
    /// Title Button에 사용할 메서드
    /// </summary>
    public void Title()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        ranking.SetActive(false);
    }

    /// <summary>
    /// Ranking Button에 사용할 메서드
    /// </summary>
    public void Rank()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        board.SetActive(false);
        ranking.SetActive(true);
    }

    /// <summary>
    /// 광고 Button에 사용할 메서드
    /// </summary>
    public void ViewAD()
    {
        // Manager를 가져온다.
        var adManager = Locator<AdManager>.Get();
        var soundManager = Locator<SoundManager>.Get();

        // 어떤 보상을 넣을 지 만들고 광고를 보여준다.
        IReward scoreReward = new ScoreReward(1.5f);
        soundManager.DestoryBGM();
        adManager.LoadRewardedAd();

        // True면 광고를 본 것이다. 그렇지 않으면 안 본 것이거나, 아직 Load가 안 된 것이다.
        if (adManager.ShowRewardedAd(scoreReward))
        {
            adButton.SetActive(false);
        }
        else
            adButton.SetActive(true);
    }
}
