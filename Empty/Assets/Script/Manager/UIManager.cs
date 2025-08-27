using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

public class UIManager
{
    // prefab은 이렇게 불러오고
    private UIPrefabCategory category;
    private Dictionary<AssetReferenceGameObject, GameObject> uiObjects = null;
    
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
        uiObjects = new Dictionary<AssetReferenceGameObject, GameObject>();
    }
    #endregion

    public AssetReferenceGameObject GetUIPrefab(UIPrefab type)
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
            return LoadResource(uiPrefab);
        }
    }

    // await을 사용하려면 async가 있어야해. async를 사용하려면 return 값이 존재한다면 Task<GameObject> 로 바꿔야 한다.
    // async await을 사용하려면 그에 맞게 코드도 바꿔야 하지만, 그렇게 되면 위에 작성한 GetUIPrefabObject도 바꿔야 해서 WaitForCompletion()을 사용하기로 했다.
    // 실행하면 중간에 렉에 해당하는 멈추는 현상이 존재할 수 있다.
    private GameObject LoadResource(AssetReferenceGameObject uiPrefab)
    {
        var uiObject = Addressables.InstantiateAsync(uiPrefab).WaitForCompletion();
        uiObjects.Add(uiPrefab, uiObject);
        return uiObject;
    }

    public string GetScore() => score;
    public void SetScore(string _score) => score = _score;

    public Level GetDegree() => degree;
    public void SetDegree(Level _degree) => degree = _degree;
}
