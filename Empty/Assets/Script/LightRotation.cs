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
            Debug.Log("Rotation is Change.");
            previousRotation = this.transform.rotation;
        }
    }
}
