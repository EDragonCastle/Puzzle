using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoticeObject : MonoBehaviour, IPointerDownHandler
{
    public class ClickEventArgs : EventArgs
    {
        public GameObject ClikedObject { get; }
        public ClickEventArgs(GameObject obj) => ClikedObject = obj;
    }

    public static event EventHandler<ClickEventArgs> Click;
    public void OnPointerDown(PointerEventData eventData)
    {
        var log = Locator.GetLogManager();
        log.Info("Clicked Image " + this.gameObject.name);

        // Board�� �ִ� SelectElement �Լ��� �� Object�� �־�� �Ѵ�.
        Click?.Invoke(this, new ClickEventArgs(this.gameObject));
    }
}
