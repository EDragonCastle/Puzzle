using UnityEngine;


public class ElementCategory : MonoBehaviour
{
    [SerializeField]
    private GameObject redElement;
    [SerializeField]
    private GameObject blueElement;
    [SerializeField]
    private GameObject greenElement;
    [SerializeField]
    private GameObject yellowElement;

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