using UnityEngine;
using UnityEngine.UI;

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

    // ���̵��� ���� ��ȭ�� �����ؾ� �Ѵ�.
    // ���� ������ ����̴�. �̸� �����صΰ� setActive true, false�� ���ָ� �ȴ�.
    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        board.SetActive(true);
        rank.SetActive(false);
    }

    public void Rank()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        rank.SetActive(true);
    }

    public void GameExit()
    {
        // ���� ����
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

public enum Level
{
    Easy,
    Normal,
    Hard,
}