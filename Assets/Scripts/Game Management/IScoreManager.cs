using MatrixUtils.GenericDatatypes;

public interface IScoreManager
{
    void IncrementScore(uint amount = 1);
    void DecrementScore();
    void IncrementJumpCount();
    void DecrementJumpCount();
    void UpdateDistanceTraveled(float distanceTraveled);
    Observer<ScoreData> CurrentScoreDataObserver { get; }
    Observer<ScoreData> HighScoreDataObserver { get; }
    void GameComplete();
}