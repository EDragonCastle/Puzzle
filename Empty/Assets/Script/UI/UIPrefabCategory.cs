using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPrefabCategory : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOver;
    [SerializeField]
    private GameObject title;
    [SerializeField]
    private GameObject board;
    [SerializeField]
    private GameObject life;
    [SerializeField]
    private GameObject rank;

    public GameObject GetUIPrefab(UIPrefab prefabType)
    {
        GameObject prefab = null;
        switch(prefabType)
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

// 이러면 초기에 생성을 해주려나?