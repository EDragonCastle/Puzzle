using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    private void OnEnable()
    {
        var uiManager = Locator<UIManager>.Get();
        textMeshPro.text = uiManager.GetScore();
    }
}
