using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Material을 관리하고 있는 Manager
/// </summary>
public class MaterialManager
{
    /// <summary>
    /// Material의 Color를 정하는 함수
    /// </summary>
    /// <param name="targetObject">material을 적용할 Object</param>
    /// <param name="_color">Enum Type 색상</param>
    public void CreateMaterial(GameObject targetObject, ElementColor _color)
    {
        // UI Image를 가져온다.
        var renderer = targetObject.GetComponent<Image>();

        // Image의 Render와 Render의 Material을 확인한다.
        if (renderer != null && renderer.material != null)
        {
            // Material을 새로 만든다.
            Material instanceMaterial = new Material(renderer.material);
            renderer.material = instanceMaterial;
            
            // Shader에 있는 Outline Color와 Color 값을 정하고, Outline이 적용할 수 없게 0으로 고정시킨다.
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

    /// <summary>
    /// Outline을 활성화 시키는 함수
    /// </summary>
    /// <param name="targetObject">material을 적용할 Object</param>
    /// <param name="isEnable">활성화 여부</param>
    public void IsEnableOutline(GameObject targetObject, bool isEnable)
    {
        // UI Image를 가져온다.
        var renderer = targetObject.GetComponent<Image>();

        // Render를 확인한다.
        if (renderer == null)
        {
            Debug.LogError("Material not exist");
            return;
        }

        // 활성화 여부에 따라 Outline을 활성화할지 말지 결정한다.
        if(isEnable)
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 1.0f);
        else
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 0.0f);
    }

    /// <summary>
    /// Direction Light의 위치와 Color에 따라 Shader가 바뀌는 함수
    /// </summary>
    /// <param name="targetObject">material을 적용할 Object</param>
    /// <param name="lightInfo">바뀌고 싶은 Light 정보</param>
    public void ChangeCustomDirectionLightInfo(GameObject targetObject, LightInfo lightInfo)
    {
        // UI Image를 가져온다.
        var renderer = targetObject.GetComponent<Image>();

        // Render를 확인한다.
        if (renderer == null)
        {
            Debug.LogError("Material not exist");
            return;
        }

        // Light의 위치와 Color 값을 Material에 적용한다.
        renderer.material.SetVector("_CustomDirectionLightDirection", lightInfo.direction);
        renderer.material.SetVector("_CustomDirectionLightColor", lightInfo.color);
    }

    // Enum Type -> Color 값으로 변환해주는 함수
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

    /// <summary>
    ///  Enum Type -> String으로 변환시켜주는 함수
    /// </summary>
    /// <param name="property">Bubble Shader에서 바꾸고 싶은 값</param>
    /// <returns></returns>
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

/// <summary>
/// Bubble Shader 속성 값
/// </summary>
public enum BubbleProperty
{
    OutlineColor,
    EnableOutline,
}