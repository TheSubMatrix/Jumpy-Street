using MatrixUtils.GenericDatatypes;
using UnityEngine.Events;

public interface IScoreReader
{
    ScoreData GetHighScore();
    ScoreData GetCurrentScore();
    UnityEvent<ScoreData> OnCurrentScoreUpdated { get; }
    UnityEvent<ScoreData> OnHighScoreUpdated { get; }
}