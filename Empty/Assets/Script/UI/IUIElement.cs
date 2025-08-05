// �� �ٵ� ���⿡ RectTransform ���� ���� ������ using UnityEngine�� ����ؼ� ���е� ��¿ �� ����?
using UnityEngine;

public interface IUIElement
{
    public ElementInfo GetElementInfo();
    public void SetElementInfo(ElementInfo info);
    public RectTransform GetRectTransform();
    public GameObject GetGameObject();
    public Material GetMaterial();
}
