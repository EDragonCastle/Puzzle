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

        // Clone�� �ϸ� �� �ǰ�, ������ �ؾ� �ϴµ� ������ ���?
        // ���纻���� �ݳ��ϸ� ���� �� �� ��� ������ ������ �ݳ��ؾ� �Ѵٴ� ����� �˾Ҵ�.
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
