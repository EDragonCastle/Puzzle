using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Material�� �����ϰ� �ִ� Manager
/// </summary>
public class MaterialManager
{
    /// <summary>
    /// Material�� Color�� ���ϴ� �Լ�
    /// </summary>
    /// <param name="targetObject">material�� ������ Object</param>
    /// <param name="_color">Enum Type ����</param>
    public void CreateMaterial(GameObject targetObject, ElementColor _color)
    {
        // UI Image�� �����´�.
        var renderer = targetObject.GetComponent<Image>();

        // Image�� Render�� Render�� Material�� Ȯ���Ѵ�.
        if (renderer != null && renderer.material != null)
        {
            // Material�� ���� �����.
            Material instanceMaterial = new Material(renderer.material);
            renderer.material = instanceMaterial;
            
            // Shader�� �ִ� Outline Color�� Color ���� ���ϰ�, Outline�� ������ �� ���� 0���� ������Ų��.
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
    /// Outline�� Ȱ��ȭ ��Ű�� �Լ�
    /// </summary>
    /// <param name="targetObject">material�� ������ Object</param>
    /// <param name="isEnable">Ȱ��ȭ ����</param>
    public void IsEnableOutline(GameObject targetObject, bool isEnable)
    {
        // UI Image�� �����´�.
        var renderer = targetObject.GetComponent<Image>();

        // Render�� Ȯ���Ѵ�.
        if (renderer == null)
        {
            Debug.LogError("Material not exist");
            return;
        }

        // Ȱ��ȭ ���ο� ���� Outline�� Ȱ��ȭ���� ���� �����Ѵ�.
        if(isEnable)
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 1.0f);
        else
            renderer.material.SetFloat(BubblePropertyToString(BubbleProperty.EnableOutline), 0.0f);
    }

    /// <summary>
    /// Direction Light�� ��ġ�� Color�� ���� Shader�� �ٲ�� �Լ�
    /// </summary>
    /// <param name="targetObject">material�� ������ Object</param>
    /// <param name="lightInfo">�ٲ�� ���� Light ����</param>
    public void ChangeCustomDirectionLightInfo(GameObject targetObject, LightInfo lightInfo)
    {
        // UI Image�� �����´�.
        var renderer = targetObject.GetComponent<Image>();

        // Render�� Ȯ���Ѵ�.
        if (renderer == null)
        {
            Debug.LogError("Material not exist");
            return;
        }

        // Light�� ��ġ�� Color ���� Material�� �����Ѵ�.
        renderer.material.SetVector("_CustomDirectionLightDirection", lightInfo.direction);
        renderer.material.SetVector("_CustomDirectionLightColor", lightInfo.color);
    }

    // Enum Type -> Color ������ ��ȯ���ִ� �Լ�
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
    ///  Enum Type -> String���� ��ȯ�����ִ� �Լ�
    /// </summary>
    /// <param name="property">Bubble Shader���� �ٲٰ� ���� ��</param>
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
/// Bubble Shader �Ӽ� ��
/// </summary>
public enum BubbleProperty
{
    OutlineColor,
    EnableOutline,
}