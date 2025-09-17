using UnityEngine;

/// <summary>
/// Puzzle Element Object�� ��� �ִ� Factory��.
/// </summary>
public class Factory
{
    // ElementColor�� Elment Category�� ���� Object Pool
    private ObjectPool<ElementColor, ElementCategory> objectPools;

    // Factory ������
    #region Factory Construct
    /// <summary>
    /// Factory�� �����Ѵ�.
    /// </summary>
    /// <param name="_category">Element Category�� �ʿ��ϴ�.</param>
    /// <param name="parent">Object���� �� ���� ������ �� �ִ� �θ� ��</param>
    public Factory(ElementCategory _category, GameObject parent = null)
    {
        objectPools = new ObjectPool<ElementColor, ElementCategory>(_category, parent);
    }

    /// <summary>
    /// Factory�� �����Ѵ�.
    /// </summary>
    /// <param name="_category">Element Category�� �ʿ��ϴ�.</param>
    /// <param name="poolInitSize">Object�� Size�� ���� �ּ� ��</param>
    /// <param name="parent">Object���� �� ���� ������ �� �ִ� �θ� ��</param>
    public Factory(ElementCategory _category, int poolInitSize, GameObject parent = null )
    {
        objectPools = new ObjectPool<ElementColor, ElementCategory>(_category, poolInitSize, parent);
    }
    #endregion

    /// <summary>
    /// IUIElement(Puzzle Element)�� Return�ϴ� �Լ�
    /// </summary>
    /// <param name="color">Enum Color</param>
    /// <param name="position">��ġ</param>
    /// <param name="rotation">ȸ��</param>
    /// <param name="scale">ũ��</param>
    /// <param name="parent">�θ� ��</param>
    /// <returns>IUIElement (Puzzle Element)</returns>
    public IUIElement CreateUIObject(ElementColor color, Vector2 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        // object Pool���� color���� ���� object�� �����´�.
        GameObject elementInfo = objectPools.Get(color);
        elementInfo.transform.SetParent(parent);
        IUIElement uiInterface = elementInfo.GetComponent<IUIElement>();

        // rectTransform�� �����ͼ� ��ġ, ȸ��, ũ�⸦ �����Ѵ�.
        var elementRectTransform = uiInterface.GetRectTransform();
        elementRectTransform.anchoredPosition = position;
        elementRectTransform.rotation = rotation;
        elementRectTransform.localScale = scale;
       
        return uiInterface;
    }

    /// <summary>
    /// Object �������� �ݳ�
    /// Object Pool�� �̿��ؼ� �ٽ� �ǵ�����.
    /// </summary>
    /// <param name="_gameObject"></param>
    public void DestoryUIObject(GameObject _gameObject)
    {
        // object Pool�� �̿��ؼ� �ݳ��Ѵ�.
        objectPools.Return(_gameObject);
    }
}
