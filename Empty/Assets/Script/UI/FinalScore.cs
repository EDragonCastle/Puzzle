using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshPro;

    // 점수 등록을 위한 server 등록
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
                // 점수가 들어간다.
                Debug.Log($"New Ranking Score : {score}");
                await server.WriteNewScore("Testing", score);
            }
            else
            {
                // 들어오지 않았다.
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
