using UnityEngine;

/// <summary>
/// Game Manager는 처음에 실행되어야 한다.
/// 0702 LYS
/// </summary>
[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private LogManager logManager;

    private void Awake() {
        // Locator에 Log Manager 등록
        if(logManager != null) {
            Locator.ProvideLogManager(logManager);        
        }
        else {
            // logManager를 깜빡하고 안 넣어서 없을 수도 있잖아.
            // 그걸 방지 하기 위한 것이 필요한데 나중에 추가하자.
        }
    }
}
