Resource Manager 설명

Empty\Assets\Script\Manager\ResourceManager.cs 파일 참고
ResourceManager를 생성하면 생성자에서 Load를 실행합니다.
Load 부분에서는 ResourceType과 이름을 가지고 Addressable에서 해당 Resource를 메모리에 Load 합니다.
가져올때는 GetResource(ResourceType, string)으로 찾는데, 어느 type인지 몰라도 이름만 알고 있으면 해당 Resource를 찾을 수 있습니다.
그리고 더 이상 사용하지 않는다면 원본 Data를 Release로 반납하면 됩니다.

Load를 했다면 기존에 [Serialize] private GameObject data;와 같이 해당 data를 가지고 GameObject.Instaniate를 사용해서 생성할 수 있습니다.


Object Pool 수정 사항
기존에는 IUIElement에 해당하는 object만 Object Pool을 사용할 수 있었습니다.
하지만 Sound도 Object Pool을 사용해야 하기 때문에 object Pool을 수정하기로 했습니다. 
<T, CATEGORY>에서 T는 Enum으로 설정된 변수를 의미합니다. ex) ElementColor, SFX (이펙트의 이름을 Enum으로 적는다면 이 곳)
CATEGORY는 ElementCategory, SoundCategory를 말합니다.
추가로 Object Pool을 Customizing을 하고 싶으면, if (typeof(CATEGORY) == typeof(ElementCategory) && typeof(T) == typeof(ElementColor)) 예시와 같이
typeof keyword를 사용해서 기존 코드와 분리해서 사용할 수 있습니다.
