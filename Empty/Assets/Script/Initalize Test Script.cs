using UnityEngine;

public class InitalizeTestScript : MonoBehaviour
{
    void Start() {
        var log = Locator.GetLogManager();
        log.Fatal("Test Fatal");
    }

    void Update() {
        
    }
}
