using UnityEngine;

public class Factory
{
    private ElementCategory category;
    private ObjectPool objectPools;

    #region Factory Construct
    public Factory(ElementCategory _category, GameObject parent = null)
    {
        category = _category;
        objectPools = new ObjectPool(_category, parent);
    }

    public Factory(ElementCategory _category, int poolInitSize, GameObject parent = null )
    {
        category = _category;
        objectPools = new ObjectPool(_category, poolInitSize, parent);
    }
    #endregion

    // 이건 일반 Object를 생성할 때나 좋은 방식이다. UI 생성은 좋은 방식이 아니여서 바꿔야 한다.
    public IUIElement CreateUIObject(ElementColor color, Vector2 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        // 나중에 Object Pool로 교체할 것.
        GameObject elementInfo = objectPools.Get(color);
        elementInfo.transform.SetParent(parent);
        IUIElement uiInterface = elementInfo.GetComponent<IUIElement>();

        // UI Rect Transform Setting 이렇게 하면 Elment 안에 RectTransform이랑 겹치려나? 없애서 괜찮아.
        var elementRectTransform = uiInterface.GetRectTransform();
        elementRectTransform.anchoredPosition = position;
        elementRectTransform.rotation = rotation;
        elementRectTransform.localScale = scale;
       
        return uiInterface;
    }

    // 이 GameObject를 가지고 반납을 해야하는데 어떻게 쉽게할 수 있을까?
    public void DestoryUIObject(GameObject _gameObject)
    {
        // object Pool을 이용해서 삭제해야 한다.
        objectPools.Return(_gameObject);
    }
}
