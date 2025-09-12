using UnityEngine;

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
    // Update is called once per frame
    void Update()
    {
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

public struct LightInfo
{
    public Vector4 color;
    public Vector4 direction;
}
