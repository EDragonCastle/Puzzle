public class PlayerScore
{
    public string userId;
    public int score;

    public PlayerScore() { }
    public PlayerScore(string _userId, int _score)
    {
        this.userId = _userId;
        this.score = _score;
    }
}