using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestAddressable : MonoBehaviour
{
    // Local로 일단 Inspector로 관리해서 사용해보는게 좋지 않을까?
    // Prefab 처럼 그냥 Instaniate처럼 사용은 못한다.
    // Instaniate하고 Result를 해야 원하는 Type을 사용할 수 있다.
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

    // 음.. 알 것 같기도 하고? 해보자.
    private void Update()
    {
        // 생성
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            soundObject[0].InstantiateAsync().Completed += (clip) =>
            {
                testSoundObject = clip.Result;
            };

            GetObject();
        }

        // 반납
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
