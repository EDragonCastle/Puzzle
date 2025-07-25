using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private GameObject title;
    private GameObject gameOver;
    private GameObject board;
    private GameObject ranking;

    private void Start()
    {
        var uiManager = Locator<UIManager>.Get();
        title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
        board = uiManager.GetUIPrefabObject(UIPrefab.Board);
        ranking = uiManager.GetUIPrefabObject(UIPrefab.Rank);
    }


    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        ranking.SetActive(false);
        board.SetActive(true);
    }

    public void Title()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
        ranking.SetActive(false);
    }

    public void Rank()
    {
        title.SetActive(false);
        gameOver.SetActive(true);
        board.SetActive(false);
        ranking.SetActive(true);
    }

    // 아 근데 setactive true, false 이거 마음에 안 드네
    // 일단 보류하고 지금은 랭킹이나 신경쓰자.
}
