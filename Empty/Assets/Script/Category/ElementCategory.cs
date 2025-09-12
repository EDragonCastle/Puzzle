using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ElementCategory
{
    #region Element Object
    private GameObject redElement;
    private GameObject blueElement;
    private GameObject greenElement;
    private GameObject yellowElement;
    #endregion

    // Resource Manager���� Load�� �Ϸ�Ǿ���.
    // ���� �޾ƿͼ� ����ϸ� �ȴ�. ��� �޾ƾ� �ұ�? ������ 4���� ���� �ۿ� ������ Object 4���� �־�δ°� ���� �� ����.
    public ElementCategory(GameObject red, GameObject blue, GameObject green, GameObject yellow)
    {
        redElement = red;
        blueElement = blue;
        greenElement = green;
        yellowElement = yellow;
    }

    public GameObject GetCategory(ElementColor _category)
    {
        GameObject categoryObject = null;
        switch (_category)
        {
            case ElementColor.Red:
                categoryObject = redElement;
                break;
            case ElementColor.Blue:
                categoryObject = blueElement;
                break;
            case ElementColor.Green:
                categoryObject = greenElement;
                break;
            case ElementColor.Yellow:
                categoryObject = yellowElement;
                break;
            default:
                categoryObject = null;
                break;
        }

        if (categoryObject == null)
        {
            Debug.Log($"{_category} exist None Object");
            return null;
        }

        return categoryObject;
    }
}

public enum ElementColor
{
    Red,
    Blue,
    Green,
    Yellow,
    End,
}


// Board����
/*
    // color �����̳� �湮���γ� ��ġ�� �������ش�. 
    // ���⼭ �ٽ��� color�ε�, ������ �̹� ������ ���� ���� �ʿ��ұ�? �ʿ����� �ʴٰ� ����.
    var newElementInfo = new ElementInfo();

    newElementInfo.color = randIndex;
    newElementInfo.isVisits = false;
    newElementInfo.position = new Vector2(x, 0);
    newElementUIPos.anchoredPosition = upperStartPos;
 */