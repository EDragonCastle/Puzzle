using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(RectTransform))]
public class Element : MonoBehaviour, IPointerDownHandler, IUIElement, IChannel
{
    [SerializeField]
    private ElementInfo elementInfo;
    private RectTransform rectTransform;

    #region IUIElement Interface Methord
    public ElementInfo GetElementInfo() => elementInfo;
    public void SetElementInfo(ElementInfo info) => elementInfo = info;
    public RectTransform GetRectTransform() => rectTransform;
    public GameObject GetGameObject() => this.gameObject; 
    #endregion

    public void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        var materialManager = Locator<MaterialManager>.Get();
        materialManager.ChangeCustomDirectionLightInfo(this.gameObject, new LightInfo() { color = Vector4.one, direction = Vector4.zero });
        eventManager.Subscription(ChannelInfo.LightInfo, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.LightInfo, HandleEvent);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{elementInfo.position.x} {elementInfo.position.y}");
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Select, this.gameObject);
        var soundManager = Locator<SoundManager>.Get();
        soundManager.PlaySFX(SFX.Pop3);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.LightInfo:
                if(information is LightInfo lightInfo)
                    ChangeLightMaterial(lightInfo);
                break;
        }
        
    }

    private void ChangeLightMaterial(LightInfo lightInfo)
    {
        var materialManager = Locator<MaterialManager>.Get();
        materialManager.ChangeCustomDirectionLightInfo(this.gameObject, lightInfo);
    }
}

[System.Serializable]
public struct ElementInfo
{
    public Vector2 position;
    public int color;
    public bool isVisits;
}
