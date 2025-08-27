using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class UIPrefabCategory : MonoBehaviour
{
    [SerializeField]
    private AssetReferenceGameObject gameOver;
    [SerializeField]
    private AssetReferenceGameObject title;
    [SerializeField]
    private AssetReferenceGameObject board;
    [SerializeField]
    private AssetReferenceGameObject life;
    [SerializeField]
    private AssetReferenceGameObject rank;

    public AssetReferenceGameObject GetUIPrefab(UIPrefab prefabType)
    {
        AssetReferenceGameObject prefab = null;
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