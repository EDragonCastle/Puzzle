using UnityEngine;

/// <summary>
/// Unity에서 Log Level이 없어서 만든 Manager다.
/// 07-02 LYS
/// </summary>
public class LogManager : MonoBehaviour
{
    // 1. Color도 Custom 할 수 있으면 좋겠다.
    // 2. 그리고 Debug에서만 실행됐으면 좋겠어.
    // 3. Release에서는 실행되서는 안 돼!
    // 4. 그렇다고 Resource Folder에 넣을 필요는 없는 것 같아.
    // 4 Reason 게임 내에서는 필요없는 기능이기 때문이지!
    // 5. Editor 상에서만 필요한 기능이야.

    public void Fatal(string message) => Debug.LogError($"<color=red>[FATAL]</color> {message}");
    public void Error(string message) => Debug.LogError($"<color=red>[ERROR]</color> {message}");
    public void Warning(string message) => Debug.LogWarning($"<color=yellow>[WARNING]</color> {message}");
    public void Info(string message) => Debug.Log($"<color=white>[INFO]</color> {message}");
}
