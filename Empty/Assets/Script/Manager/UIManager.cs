using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class UIManager
{
    // prefab은 이렇게 불러오고
    private UIPrefabCategory category;
    private Dictionary<GameObject, GameObject> uiObjects = null;
    
    // ui 에 필요한 data는 이렇게 불러와야 할까?
    private string score;
    private Level degree;

    private TaskCompletionSource<bool> initializeCompleteSource;
    private Task initalizeTask => initializeCompleteSource.Task;

    #region UIManager Structor
    public UIManager(UIPrefabCategory _category)
    {
        category = _category;
        score = "";
        uiObjects = new Dictionary<GameObject, GameObject>();
    }
    #endregion

    public GameObject GetUIPrefab(UIPrefab type)
    {
        var uiPrefab = category.GetUIPrefab(type);
        return uiPrefab;
    }

    public GameObject GetUIPrefabObject(UIPrefab type)
    {
        var uiPrefab = category.GetUIPrefab(type);
        GameObject uiObject = null;

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
    public string GetScore() => score;
    public void SetScore(string _score) => score = _score;

    public Level GetDegree() => degree;
    public void SetDegree(Level _degree) => degree = _degree;
}
