using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResourceManager : MonoBehaviour
{
    ResourceManager resourceManager;

    private GameObject prefabObject;
    private GameObject testObject;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Memory Load");
        resourceManager = new ResourceManager();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Object Instaniate");
            prefabObject = resourceManager.GetResource(ResourceType.BGM, "Stage1");

            if(prefabObject != null)
                testObject = Instantiate(prefabObject);
        }

        // Clone을 하면 안 되고, 원본을 해야 하는데 원본을 어떻게?
        // 복사본으로 반납하면 절대 알 수 없어서 무조건 원본을 반납해야 한다는 사실을 알았다.
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Memory Release");
            resourceManager.ReleaseResource(prefabObject);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Object Destory");
            Destroy(testObject);
        }
    }



}
