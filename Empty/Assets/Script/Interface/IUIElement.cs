using UnityEngine;

/// <summary>
/// Puzzle Object를 담당하고 있는 Interface
/// </summary>
public interface IUIElement
{
    public ElementInfo GetElementInfo();
    public void SetElementInfo(ElementInfo info);
    public RectTransform GetRectTransform();
    public GameObject GetGameObject();
}
