using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 각종 UI들을 관리하고 있는 Manager
/// </summary>
public class UIManager
{
    // ui category를 담고 있다.
    private UIPrefabCategory category;

    // key: origine, value : clone object
    private Dictionary<GameObject, GameObject> uiObjects = null;
    
    // Score 점수를 저장하고 있다.
    private string score;
    private Level degree;

    #region UIManager Structor
    /// <summary>
    /// UIManager 생성자
    /// </summary>
    /// <param name="_category">UI category</param>
    public UIManager(UIPrefabCategory _category)
    {
        category = _category;
        score = "";
        uiObjects = new Dictionary<GameObject, GameObject>();
    }
    #endregion

    /// <summary>
    /// UI Origine Prefab을 가져온다.
    /// </summary>
    /// <param name="type">UI Prefab Enum Type</param>
    /// <returns>Prefab object</returns>
    public GameObject GetUIPrefab(UIPrefab type)
    {
        var uiPrefab = category.GetUIPrefab(type);
        return uiPrefab;
    }

    /// <summary>
    /// UI Clone object를 가져온다.
    /// </summary>
    /// <param name="type">UI Prefabe Enum Type</param>
    /// <returns>Clone Object</returns>
    public GameObject GetUIPrefabObject(UIPrefab type)
    {
        // Category에서 Origine Prefab을 가져온다.
        var uiPrefab = category.GetUIPrefab(type);
        GameObject uiObject = null;

        // origine Prefab Object를 통해 확인해서 있으면 Clone Object를 출력하고, 없다면 새로 만들어서 생성한다.
        if (uiObjects.ContainsKey(uiPrefab))
        {
            return uiObjects[uiPrefab];
        }
        else
        {
            uiObject = GameObject.Instantiate(uiPrefab);
            uiObjects.Add(uiPrefab, uiObject);
            return uiObject;
        }
    }

    // Score를 조절할 수 있는 메서드
    public string GetScore() => score;
    public void SetScore(string _score) => score = _score;

    // 난이도를 조절하는 곳 (실제 사용하지는 않는다)
    public Level GetDegree() => degree;
    public void SetDegree(Level _degree) => degree = _degree;
}
