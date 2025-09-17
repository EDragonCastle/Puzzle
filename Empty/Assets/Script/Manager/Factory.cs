using UnityEngine;

/// <summary>
/// Puzzle Element Object를 담고 있는 Factory다.
/// </summary>
public class Factory
{
    // ElementColor와 Elment Category를 담은 Object Pool
    private ObjectPool<ElementColor, ElementCategory> objectPools;

    // Factory 생성자
    #region Factory Construct
    /// <summary>
    /// Factory를 생성한다.
    /// </summary>
    /// <param name="_category">Element Category가 필요하다.</param>
    /// <param name="parent">Object들을 한 곳에 보관할 수 있는 부모 값</param>
    public Factory(ElementCategory _category, GameObject parent = null)
    {
        objectPools = new ObjectPool<ElementColor, ElementCategory>(_category, parent);
    }

    /// <summary>
    /// Factory를 생성한다.
    /// </summary>
    /// <param name="_category">Element Category가 필요하다.</param>
    /// <param name="poolInitSize">Object를 Size를 정할 최소 값</param>
    /// <param name="parent">Object들을 한 곳에 보관할 수 있는 부모 값</param>
    public Factory(ElementCategory _category, int poolInitSize, GameObject parent = null )
    {
        objectPools = new ObjectPool<ElementColor, ElementCategory>(_category, poolInitSize, parent);
    }
    #endregion

    /// <summary>
    /// IUIElement(Puzzle Element)를 Return하는 함수
    /// </summary>
    /// <param name="color">Enum Color</param>
    /// <param name="position">위치</param>
    /// <param name="rotation">회전</param>
    /// <param name="scale">크기</param>
    /// <param name="parent">부모 값</param>
    /// <returns>IUIElement (Puzzle Element)</returns>
    public IUIElement CreateUIObject(ElementColor color, Vector2 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        // object Pool에서 color값을 통해 object를 가져온다.
        GameObject elementInfo = objectPools.Get(color);
        elementInfo.transform.SetParent(parent);
        IUIElement uiInterface = elementInfo.GetComponent<IUIElement>();

        // rectTransform을 가져와서 위치, 회전, 크기를 조절한다.
        var elementRectTransform = uiInterface.GetRectTransform();
        elementRectTransform.anchoredPosition = position;
        elementRectTransform.rotation = rotation;
        elementRectTransform.localScale = scale;
       
        return uiInterface;
    }

    /// <summary>
    /// Object 삭제이자 반납
    /// Object Pool을 이용해서 다시 되돌린다.
    /// </summary>
    /// <param name="_gameObject"></param>
    public void DestoryUIObject(GameObject _gameObject)
    {
        // object Pool을 이용해서 반납한다.
        objectPools.Return(_gameObject);
    }
}
