using UnityEngine;

/// <summary>
/// Title을 담당하고 있는 Class
/// </summary>
public class Title : MonoBehaviour
{
    private GameObject title;
    private GameObject gameOver;
    private GameObject board;
    private GameObject rank;

    private UIManager uiManager;

    private void Awake()
    {
        uiManager = Locator<UIManager>.Get();
        title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
        board = uiManager.GetUIPrefabObject(UIPrefab.Board);
        rank = uiManager.GetUIPrefabObject(UIPrefab.Rank);

        // 일단 기본값
        uiManager.SetDegree(Level.Easy);
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        rank.SetActive(false);
    }

    /// <summary>
    /// 난이도 Button를 생각해서 만든 메서드 (현재는 사용하지 않는다.)
    /// </summary>
    public void Easy()
    {
        uiManager.SetDegree(Level.Easy);
        GameStart();
    }


    public void Normal()
    {
        uiManager.SetDegree(Level.Normal);
        GameStart();
    }

    public void Hard()
    {
        uiManager.SetDegree(Level.Hard);
        GameStart();
    }
    
    /// <summary>
    /// 게임 시작 Button을 누르면 실행할 메서드
    /// </summary>
    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        board.SetActive(true);
        rank.SetActive(false);
    }

    /// <summary>
    /// Ranking Button을 누르면 실행할 메서드
    /// </summary>
    public void Rank()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        rank.SetActive(true);
    }

    /// <summary>
    /// Game Exit Button을 누르면 실행할 메서드
    /// </summary>
    public void GameExit()
    {
        // 게임 종료
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

/// <summary>
/// 난이도를 담고 있는 Enum Type
/// </summary>
public enum Level
{
    Easy,
    Normal,
    Hard,
}