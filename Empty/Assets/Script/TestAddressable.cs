using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestAddressable : MonoBehaviour
{
    // Local�� �ϴ� Inspector�� �����ؼ� ����غ��°� ���� ������?
    // Prefab ó�� �׳� Instaniateó�� ����� ���Ѵ�.
    // Instaniate�ϰ� Result�� �ؾ� ���ϴ� Type�� ����� �� �ִ�.
    [SerializeField]
    private AssetReferenceGameObject[] animationCharacter;

    [SerializeField]
    private AssetReferenceGameObject[] element;

    [SerializeField]
    private AssetReferenceGameObject[] soundObject;

    [SerializeField]
    private AssetReferenceGameObject[] uiObject;
    private AssetReferenceGameObject noneObject;


    [SerializeField]
    private GameObject testSoundObject;
    
    [SerializeField]
    private GameObject parentObject;

    // ��.. �� �� ���⵵ �ϰ�? �غ���.
    private void Update()
    {
        // ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            soundObject[0].InstantiateAsync().Completed += (clip) =>
            {
                testSoundObject = clip.Result;
            };

            GetObject();
        }

        // �ݳ�
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if(testSoundObject != null)
            {
                Addressables.ReleaseInstance(testSoundObject);
                testSoundObject = null;
            }
        }
    }


    private GameObject GetObject()
    {
        GameObject newObejct = null;
        element[0].InstantiateAsync(parentObject.transform).Completed += (obj) =>
        {
            newObejct = obj.Result;
        };

        return newObejct;
    }
}
