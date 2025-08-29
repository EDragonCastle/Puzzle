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

    // Resource Manager에서 Load가 완료되었다.
    // 이제 받아와서 사용하면 된다. 어떻게 받아야 할까? 지금은 4가지 색상 밖에 없으니 Object 4개를 넣어두는게 좋을 것 같다.
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


// Board에서
/*
    // color 세팅이나 방문여부나 위치를 조절해준다. 
    // 여기서 핵심은 color인데, 위에서 이미 정보를 만들어서 굳이 필요할까? 필요하지 않다고 본다.
    var newElementInfo = new ElementInfo();

    newElementInfo.color = randIndex;
    newElementInfo.isVisits = false;
    newElementInfo.position = new Vector2(x, 0);
    newElementUIPos.anchoredPosition = upperStartPos;
 */