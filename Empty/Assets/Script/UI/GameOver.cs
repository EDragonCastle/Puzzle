using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private GameObject title;
    private GameObject gameOver;
    private GameObject board;

    private void Start()
    {
        var uiManager = Locator<UIManager>.Get();
        title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
        board = uiManager.GetUIPrefabObject(UIPrefab.Board);
    }


    public void GameStart()
    {
        title.SetActive(false);
        gameOver.SetActive(false);
        board.SetActive(true);
    }

    public void Title()
    {
        title.SetActive(true);
        gameOver.SetActive(false);
        board.SetActive(false);
    }
}
