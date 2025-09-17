using UnityEngine;

/// <summary>
/// Puzzle Object�� ����ϰ� �ִ� Interface
/// </summary>
public interface IUIElement
{
    public ElementInfo GetElementInfo();
    public void SetElementInfo(ElementInfo info);
    public RectTransform GetRectTransform();
    public GameObject GetGameObject();
}
