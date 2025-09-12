using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIPrefabCategory
{ 
    #region UIObject
    private GameObject gameOver;
    private GameObject title;
    private GameObject board;
    private GameObject life;
    private GameObject rank;
    #endregion

    
    public UIPrefabCategory(GameObject _title, GameObject _board, GameObject _life, GameObject _rank, GameObject _gameOver)
    {
        title = _title;
        board = _board;
        life = _life;
        rank = _rank;
        gameOver = _gameOver;
    }

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

public enum UIPrefab
{
    Title,
    Board,
    Gameover,
    Rank,
    Life,
    End,
}
