/// <summary>
/// Ranking에 사용할 Player Score 정보를 담고 있는 class
/// </summary>
public class PlayerScore
{
    public string userId;
    public int score;

    #region PlayerScore 생성자
    public PlayerScore() { }
    
    public PlayerScore(string _userId, int _score)
    {
        this.userId = _userId;
        this.score = _score;
    }
    #endregion
}