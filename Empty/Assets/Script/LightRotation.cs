using UnityEngine;

/// <summary>
/// Direction Light ������ �����ϰ� �ִ� Class
/// </summary>
[RequireComponent(typeof(Light))]
public class LightRotation : MonoBehaviour
{
    private Light light;
    private Quaternion previousRotation;

    private void Start()
    {
        light = this.GetComponent<Light>();
        previousRotation = this.transform.rotation;
    }

    void Update()
    {
        // Rotion ������ �����ϰ� �ִ�. -> �� Frame���� Ȯ���ϰ� �־ ���� �ʴ�.
        if(previousRotation != this.transform.rotation)
        {
            var eventManager = Locator<EventManager>.Get();
            var lightInfo = new LightInfo()
            {
                color = light.color,
                direction = -light.transform.forward
            };
            
            previousRotation = this.transform.rotation;
            eventManager.Notify(ChannelInfo.LightInfo, lightInfo);
        }
    }
}

/// <summary>
/// Light ������ ��� �ִ� Struct
/// </summary>
public struct LightInfo
{
    public Vector4 color;
    public Vector4 direction;
}
