// 아 근데 여기에 RectTransform 좋지 않은 이유가 using UnityEngine을 사용해서 별론데 어쩔 수 없나?
using UnityEngine;

public interface IUIElement
{
    public ElementInfo GetElementInfo();
    public void SetElementInfo(ElementInfo info);
    public RectTransform GetRectTransform();
    public GameObject GetGameObject();
    public Material GetMaterial();
}
