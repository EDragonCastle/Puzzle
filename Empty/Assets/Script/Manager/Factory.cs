using UnityEngine;

public class Factory
{
    private ElementCategory category;

    public Factory(ElementCategory _category)
    {
        category = _category;
    }

    // 이건 일반 Object를 생성할 때나 좋은 방식이다. UI 생성은 좋은 방식이 아니여서 바꿔야 한다.
    public IUIElement CreateUIObject(ElementColor color, Vector2 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        // color를 이용해서 object를 생성한다.
        GameObject element = category.GetCategory(color);
        
        // 나중에 Object Pool로 교체할 것.
        GameObject elementInfo = Object.Instantiate(element, parent);
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
        GameObject.Destroy(_gameObject);
    }
}

// Factory를 이용해서 Object를 만든다.
// Object를 어떻게 만들 것인지는 지금은 Enum으로 편하게 만들고 싶다는 것이다.
// 생각 정리가 필요할 것 같다.
// 지금 현재 Element Script가 있어.
// 이미 여기서 다 조절하고 있어. Color랑 ElementInfo랑 
// 근데 Factory를 사용하기 위해서 바꿔야 하는데 어떻게 바꿔야 할까??

// 현재 Object 생성하는 방식
// var newElement = Instantiate(prefab[randIndex]);
// newElement.transform.SetParent(board.transform, false);

// 이 부분을 바꾸고 싶다해서 Factory로 바꾼 것이잖아?
// 저 위에 CreateUIObject를 이용해서 생성하면 되잖아.

// 근데 위에 생성 방식을 사용하면 Random 방식은 되지 않고, Object 생성에서 Position과 Rotation은 필요 없다.
// 왜냐하면 UI는 Transform을 사용하는 것이 아니라 anchoredPosition을 사용하는 거니까
// Rect Transform Vector2

// IUIElement 에서 GetElementInfo, RectTransform을 get, set 하는 건 어때
// -> 실 사용자는 무조건 elementInfo랑 RectTransform을 구현해야 해.
// get, set을 interface로 반드시 구현해야 한다 하면 Element를 받아올 필요없잖아.
// 근데 set은 필요 없을듯 어차피 Transform이 Ref Type이니까 
// IUIElement에서 받아오면 되니까?

// 그러면 이름도 Element이지만 나중에 추가할 것들은 이름이 Element일 필요는 없지
// 핵심 뭐 터지거나 그런건 board 에서 해야하지 않을까?