using UnityEngine;

/// <summary>
/// UI ������ ��� �ִ� Category
/// </summary>
public class UIPrefabCategory
{ 
    #region UIObject
    private GameObject gameOver;
    private GameObject title;
    private GameObject board;
    private GameObject life;
    private GameObject rank;
    #endregion

    /// <summary>
    /// UIPrefabCategory ������
    /// </summary>
    /// <param name="_title">Title object</param>
    /// <param name="_board">Board object</param>
    /// <param name="_life">Life object</param>
    /// <param name="_rank">Rank object</param>
    /// <param name="_gameOver">Gameover object</param>
    public UIPrefabCategory(GameObject _title, GameObject _board, GameObject _life, GameObject _rank, GameObject _gameOver)
    {
        title = _title;
        board = _board;
        life = _life;
        rank = _rank;
        gameOver = _gameOver;
    }

    /// <summary>
    /// UIPrefab ������ Object ���
    /// </summary>
    /// <param name="prefabType">UIPrefab ����</param>
    /// <returns>UIPrefab Object</returns>
    public GameObject GetUIPrefab(UIPrefab prefabType)
    {
        GameObject prefab = null;

        switch (prefabType)
        {
            case UIPrefab.Title:
                prefab = title;
                break;
            case UIPrefab.Board:
                prefab = board;
                break;
            case UIPrefab.Gameover:
                prefab = gameOver;
                break;
            case UIPrefab.Life:
                prefab = life;
                break;
            case UIPrefab.Rank:
                prefab = rank;
                break;
        }

        return prefab;
    }
}

/// <summary>
/// UIPrefab ������ ����ϰ� �ִ� Enum
/// </summary>
public enum UIPrefab
{
    Title,
    Board,
    Gameover,
    Rank,
    Life,
    End,
}
