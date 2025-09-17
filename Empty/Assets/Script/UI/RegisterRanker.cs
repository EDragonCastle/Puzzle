using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;


/// <summary>
/// Ranker�� ����� ����ϴ� Class
/// </summary>
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
    
    /// <summary>
    /// Ranker�� ����Ѵ�.
    /// </summary>
    public async void RegisterRankerName()
    {
        // ��ĭ �˻�
        if (string.IsNullOrWhiteSpace(customUserName.text))
        {
            waringPanelObject.SetActive(true);
            warningText.text = "Rename User Name";
            customUserName.text = "";
            return;
        }

        // ��� ���;� �Ѵ�.
        string english = @"^[a-zA-Z0-9]*$";

        // ��� �˻��Ѵ�.
        if (!Regex.IsMatch(customUserName.text, english))
        {
            waringPanelObject.SetActive(true);
            warningText.text = "Writing User Name English";
            customUserName.text = "";
            return;
        }

        rankerObject.SetActive(false);

        // ui Manager�� board�� �̵��ϴ°� ���� ��������? �׳� �ʿ��� ������ �ϴ°� ���� �� ����.
        var uiManager = Locator<UIManager>.Get();
        var score = uiManager.GetScore();

        // Ranker�� ����Ѵ�.
        Debug.Log($"{customUserName.text} {score} is Ranking Register");
        var EDCServer = Locator<EDCServer>.Get();
        await EDCServer.WriteNewScore(customUserName.text, int.Parse(score));

        var title = uiManager.GetUIPrefabObject(UIPrefab.Title);
        var gameOver = uiManager.GetUIPrefabObject(UIPrefab.Gameover);
        var rank = uiManager.GetUIPrefabObject(UIPrefab.Rank);

        // Ranking UI�� �̵��Ѵ�.
        title.SetActive(false);
        gameOver.SetActive(false);
        rank.SetActive(true);
        
        customUserName.text = "";
    }
}
