using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;


public class RegisterRanker : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField customUserName;

    [SerializeField]
    private GameObject waringPanelObject;

    [SerializeField]
    private TMP_Text warningText;

    [SerializeField]
    private GameObject rankerObject;
    
    public async void RegisterRankerName()
    {
        // 빈칸 검사
        if (string.IsNullOrWhiteSpace(customUserName.text))
        {
            waringPanelObject.SetActive(true);
            warningText.text = "Rename User Name";
            customUserName.text = "";
            return;
        }

        // 영어만 나와야 한다.
        string english = @"^[a-zA-Z0-9]*$";

        if (!Regex.IsMatch(customUserName.text, english))
        {
            waringPanelObject.SetActive(true);
            warningText.text = "Writing User Name English";
            customUserName.text = "";
            return;
        }

        rankerObject.SetActive(false);

        // ui Manager로 board로 이동하는게 낫지 않으려나? 그냥 필요할 때마다 하는게 좋을 것 같다.
        var uiManager = Locator<UIManager>.Get();
        var score = uiManager.GetScore();

        Debug.Log($"{customUserName.text} {score} is Ranking Register");
        var EDCServer = Locator<EDCServer>.Get();
        await EDCServer.WriteNewScore(customUserName.text, int.Parse(score));

        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        var gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
        var rank = uiManager.GetUIPrefabObject(UIPrefab.Rank);

        title.SetActive(false);
        gameOver.SetActive(false);
        rank.SetActive(true);
        
        customUserName.text = "";
    }
}
