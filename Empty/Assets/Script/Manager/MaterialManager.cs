using UnityEngine;
using UnityEngine.UI;

public class MaterialManager
{
    // 외부에서 생성하도록 하자. 지금은 하나지만 나중에는 생성하는 방식이 여러 개 있을 것이다.
    public void CreateMaterial(GameObject targetObject, ElementColor _color)
    {
        var renderer = targetObject.GetComponent<Image>();

        if (renderer != null && renderer.material != null)
        {
            Material instanceMaterial = new Material(renderer.material);
            renderer.material = instanceMaterial;
            renderer.material.SetColor(BubblePropertyToString(BubbleProperty.OutlineColor), ElementColorToColor(_color));
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 0.0f);
        }
        else
        {
            if (renderer.material == null)
            {
                Debug.LogError($"Material not found on the {targetObject}");
            }
        }
    }

    // set shader setting
    public void IsEnableOutline(GameObject targetObject, bool isEnable)
    {
        var renderer = targetObject.GetComponent<Image>();

        if (renderer == null)
        {
            Debug.LogError("Material not exist");
            return;
        }
        

        if(isEnable)
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 1.0f);
        else
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 0.0f);
    }

    private Color ElementColorToColor(ElementColor _color)
    {
        Color newColor = new Color();

        switch (_color)
        {
            case ElementColor.Red:
                newColor = Color.red;
                break;
            case ElementColor.Blue:
                newColor = Color.blue;
                break;
            case ElementColor.Green:
                newColor = Color.green;
                break;
            case ElementColor.Yellow:
                newColor = Color.yellow;
                break;
            case ElementColor.End:
                Debug.LogError("None Color");
                break;
            default:
                Debug.LogError("None Color");
                break;
        }

        return newColor;
    }


    private string BubblePropertyToString(BubbleProperty property)
    {
        string propertiesName = default;

        switch(property)
        {
            case BubbleProperty.OutlineColor:
                propertiesName = "_OutlineColor";
                break;
            case BubbleProperty.EnableOutline:
                propertiesName = "_OutlineMode";
                break;
            default:
                Debug.LogError("None Bubble Property");
                break;
        }
        return propertiesName;
    }
}

public enum BubbleProperty
{
    OutlineColor,
    EnableOutline,
}