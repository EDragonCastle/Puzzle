using UnityEngine;

/// <summary>
/// GameOver�� ����ϴ� Class
/// </summary>
public class GameOver : MonoBehaviour
{
    private GameObject title;
    private GameObject gameOver;
    private GameObject board;
    private GameObject ranking;

    [SerializeField]
    private GameObject adButton;

    // �����ϸ� Manager���� ���� Object�� �����´�.
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
    /// Game Start Button�� ����� �޼���
    /// </summary>
    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        ranking.SetActive(false);
        board.SetActive(true);
    }

    /// <summary>
    /// Title Button�� ����� �޼���
    /// </summary>
    public void Title()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        ranking.SetActive(false);
    }

    /// <summary>
    /// Ranking Button�� ����� �޼���
    /// </summary>
    public void Rank()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        board.SetActive(false);
        ranking.SetActive(true);
    }

    /// <summary>
    /// ���� Button�� ����� �޼���
    /// </summary>
    public void ViewAD()
    {
        // Manager�� �����´�.
        var adManager = Locator<AdManager>.Get();
        var soundManager = Locator<SoundManager>.Get();

        // � ������ ���� �� ����� ���� �����ش�.
        IReward scoreReward = new ScoreReward(1.5f);
        soundManager.DestoryBGM();
        adManager.LoadRewardedAd();

        // True�� ���� �� ���̴�. �׷��� ������ �� �� ���̰ų�, ���� Load�� �� �� ���̴�.
        if (adManager.ShowRewardedAd(scoreReward))
        {
            adButton.SetActive(false);
        }
        else
            adButton.SetActive(true);
    }
}
