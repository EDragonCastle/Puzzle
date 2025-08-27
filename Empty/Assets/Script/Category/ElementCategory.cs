using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ElementCategory : MonoBehaviour
{
    #region Addressable Object
    [SerializeField]
    private AssetReferenceGameObject redElement;
    [SerializeField]
    private AssetReferenceGameObject blueElement;
    [SerializeField]
    private AssetReferenceGameObject greenElement;
    [SerializeField]
    private AssetReferenceGameObject yellowElement;
    #endregion

    public AssetReferenceGameObject GetCategory(ElementColor _category)
    {
        AssetReferenceGameObject categoryObject = null;
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