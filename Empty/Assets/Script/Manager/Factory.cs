using UnityEngine;

public class Factory
{
    private ElementCategory category;

    public Factory(ElementCategory _category)
    {
        category = _category;
    }

    // �̰� �Ϲ� Object�� ������ ���� ���� ����̴�. UI ������ ���� ����� �ƴϿ��� �ٲ�� �Ѵ�.
    public IUIElement CreateUIObject(ElementColor color, Vector2 position, Quaternion rotation, Vector3 scale, Transform parent = null)
    {
        // color�� �̿��ؼ� object�� �����Ѵ�.
        GameObject element = category.GetCategory(color);
        
        // ���߿� Object Pool�� ��ü�� ��.
        GameObject elementInfo = Object.Instantiate(element, parent);
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
        GameObject.Destroy(_gameObject);
    }
}

// Factory�� �̿��ؼ� Object�� �����.
// Object�� ��� ���� �������� ������ Enum���� ���ϰ� ����� �ʹٴ� ���̴�.
// ���� ������ �ʿ��� �� ����.
// ���� ���� Element Script�� �־�.
// �̹� ���⼭ �� �����ϰ� �־�. Color�� ElementInfo�� 
// �ٵ� Factory�� ����ϱ� ���ؼ� �ٲ�� �ϴµ� ��� �ٲ�� �ұ�??

// ���� Object �����ϴ� ���
// var newElement = Instantiate(prefab[randIndex]);
// newElement.transform.SetParent(board.transform, false);

// �� �κ��� �ٲٰ� �ʹ��ؼ� Factory�� �ٲ� �����ݾ�?
// �� ���� CreateUIObject�� �̿��ؼ� �����ϸ� ���ݾ�.

// �ٵ� ���� ���� ����� ����ϸ� Random ����� ���� �ʰ�, Object �������� Position�� Rotation�� �ʿ� ����.
// �ֳ��ϸ� UI�� Transform�� ����ϴ� ���� �ƴ϶� anchoredPosition�� ����ϴ� �Ŵϱ�
// Rect Transform Vector2

// IUIElement ���� GetElementInfo, RectTransform�� get, set �ϴ� �� �
// -> �� ����ڴ� ������ elementInfo�� RectTransform�� �����ؾ� ��.
// get, set�� interface�� �ݵ�� �����ؾ� �Ѵ� �ϸ� Element�� �޾ƿ� �ʿ���ݾ�.
// �ٵ� set�� �ʿ� ������ ������ Transform�� Ref Type�̴ϱ� 
// IUIElement���� �޾ƿ��� �Ǵϱ�?

// �׷��� �̸��� Element������ ���߿� �߰��� �͵��� �̸��� Element�� �ʿ�� ����
// �ٽ� �� �����ų� �׷��� board ���� �ؾ����� ������?