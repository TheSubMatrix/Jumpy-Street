using MatrixUtils.GenericDatatypes;

public interface IScoreManager
{
    void UpdateScore(uint newScore);
    void UpdateJumpCount(uint newJumpCount);
    Observer<ScoreManager.HighScoreData> CurrentScoreDataObserver { get; }
    Observer<ScoreManager.HighScoreData> HighScoreDataObserver { get; }
    void GameComplete();
}