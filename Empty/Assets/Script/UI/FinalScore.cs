using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    // ���� ����� ���� server ���
    private EDCServer server;

    private async void OnEnable()
    {
        var uiManager = Locator<UIManager>.Get();
        server = Locator<EDCServer>.Get();
        textMeshPro.text = uiManager.GetScore();

        int score = 0;
        if(int.TryParse(textMeshPro.text, out score))
        {
            /*
            if(await server.IsScoreRanker(score))
            {
                // ������ ����.
                Debug.Log($"New Ranking Score : {score}");
                await server.WriteNewScore("Testing", score);
            }
            else
            {
                // ������ �ʾҴ�.
                Debug.Log("None Puzzle Number");
            }
             */
        }
        else
        {
            Debug.LogError($"Failed to parse score");
        }
    }

}
