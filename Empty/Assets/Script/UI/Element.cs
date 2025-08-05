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
    private Material material;
    private EventManager eventManager;
    private MaterialManager materialManager;

    #region IUIElement Interface Methord
    public ElementInfo GetElementInfo() => elementInfo;
    public void SetElementInfo(ElementInfo info) => elementInfo = info;
    public RectTransform GetRectTransform() => rectTransform;
    public GameObject GetGameObject() => this.gameObject;
    public Material GetMaterial() => this.material;
    #endregion

    public void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
        eventManager = Locator<EventManager>.Get();
        material = this.GetComponent<Image>().material;
        materialManager = Locator<MaterialManager>.Get();
    }

    private void OnEnable()
    {
        //eventManager.Subscription(ChannelInfo.LightInfo, HandleEvent);
    }

    private void OnDisable()
    {
        //eventManager.Unsubscription(ChannelInfo.LightInfo, HandleEvent);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"{elementInfo.position.x} {elementInfo.position.y}");
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.Select, this.gameObject);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.LightInfo:
                
                break;
        }
    }
}

[System.Serializable]
public struct ElementInfo
{
    public Vector2 position;
    public int color;
    public bool isVisits;
}
