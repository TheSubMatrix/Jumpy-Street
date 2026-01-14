using MatrixUtils.GenericDatatypes;

public interface IScoreManager
{
    void IncrementScore(uint amount = 1);
    void DecrementScore();
    void IncrementJumpCount();
    void DecrementJumpCount();
    void UpdateDistanceTraveled(float distanceTraveled);
    Observer<ScoreManager.HighScoreData> CurrentScoreDataObserver { get; }
    Observer<ScoreManager.HighScoreData> HighScoreDataObserver { get; }
    void GameComplete();
}