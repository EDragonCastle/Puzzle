using UnityEngine;

/// <summary>
/// ��� Element ������ ��� �ִ� Class
/// </summary>
public class ElementCategory
{
    #region Element Object
    private GameObject redElement;
    private GameObject blueElement;
    private GameObject greenElement;
    private GameObject yellowElement;
    #endregion

    /// <summary>
    /// ElementCategory ������
    /// </summary>
    /// <param name="red">red element object</param>
    /// <param name="blue">blue element object</param>
    /// <param name="green">green element object</param>
    /// <param name="yellow">yellow element object</param>
    public ElementCategory(GameObject red, GameObject blue, GameObject green, GameObject yellow)
    {
        redElement = red;
        blueElement = blue;
        greenElement = green;
        yellowElement = yellow;
    }

    /// <summary>
    /// Category �������� Object�� �������� �Լ�
    /// </summary>
    /// <param name="_category">Color ����</param>
    /// <returns>element Object</returns>
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

/// <summary>
/// Eelement Color ������ ��� �ִ� Enum
/// </summary>
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