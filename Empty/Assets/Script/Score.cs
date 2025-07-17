using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;


    private void Start()
    {
        textMeshPro.text = "0";
    }

    private void OnEnable()
    {
        Board.scoreValue += ChangeScore;
    }

    private void OnDisable()
    {
        Board.scoreValue -= ChangeScore;
    }

    private void ChangeScore(int value)
    {
        int currentScore = int.Parse(textMeshPro.text);
        currentScore += value * 100;
        textMeshPro.text = currentScore.ToString();
    }
}
