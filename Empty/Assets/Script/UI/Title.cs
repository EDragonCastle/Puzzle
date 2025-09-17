using UnityEngine;

/// <summary>
/// Title�� ����ϰ� �ִ� Class
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

        // �ϴ� �⺻��
        uiManager.SetDegree(Level.Easy);
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        rank.SetActive(false);
    }

    /// <summary>
    /// ���̵� Button�� �����ؼ� ���� �޼��� (����� ������� �ʴ´�.)
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
    /// ���� ���� Button�� ������ ������ �޼���
    /// </summary>
    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        board.SetActive(true);
        rank.SetActive(false);
    }

    /// <summary>
    /// Ranking Button�� ������ ������ �޼���
    /// </summary>
    public void Rank()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        rank.SetActive(true);
    }

    /// <summary>
    /// Game Exit Button�� ������ ������ �޼���
    /// </summary>
    public void GameExit()
    {
        // ���� ����
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

/// <summary>
/// ���̵��� ��� �ִ� Enum Type
/// </summary>
public enum Level
{
    Easy,
    Normal,
    Hard,
}