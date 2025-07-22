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

    // �̰� �Ϲ� Object�� ������ ���� ���� ����̴�. UI ������ ���� ����� �ƴϿ��� �ٲ�� �Ѵ�.
    public IUIElement CreateUIObject(ElementColor color, Vector2 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        // ���߿� Object Pool�� ��ü�� ��.
        GameObject elementInfo = objectPools.Get(color);
        elementInfo.transform.SetParent(parent);
        IUIElement uiInterface = elementInfo.GetComponent<IUIElement>();

        // UI Rect Transform Setting �̷��� �ϸ� Elment �ȿ� RectTransform�̶� ��ġ����? ���ּ� ������.
        var elementRectTransform = uiInterface.GetRectTransform();
        elementRectTransform.anchoredPosition = position;
        elementRectTransform.rotation = rotation;
        elementRectTransform.localScale = scale;
       
        return uiInterface;
    }

    // �� GameObject�� ������ �ݳ��� �ؾ��ϴµ� ��� ������ �� ������?
    public void DestoryUIObject(GameObject _gameObject)
    {
        // object Pool�� �̿��ؼ� �����ؾ� �Ѵ�.
        objectPools.Return(_gameObject);
    }
}
