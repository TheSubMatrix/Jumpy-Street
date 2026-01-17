using System.IO;
using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using MatrixUtils.GenericDatatypes;
using UnityEngine;

public class ScoreManager : MonoBehaviour, IScoreManager, IDependencyProvider
{
    [Provide, UsedImplicitly]
    IScoreManager ProvideScoreManager() => this;
    
    static string HighScoreDataPath => Path.Combine(Application.persistentDataPath, "HighScore.json");
    
    static void SaveToPath(ScoreData scoreData) => File.WriteAllText(HighScoreDataPath, JsonUtility.ToJson(scoreData));
    static ScoreData ReadFromPath() => JsonUtility.FromJson<ScoreData>(File.ReadAllText(HighScoreDataPath));
    
    public void IncrementScore(uint amount = 1)
    {
        CurrentScoreDataObserver.Value.Score+= amount;
        CurrentScoreDataObserver.Notify();
        Debug.Log(CurrentScoreDataObserver.Value.Score);
    }

    public void DecrementScore()
    {
        CurrentScoreDataObserver.Value.Score--;
        CurrentScoreDataObserver.Notify();
        Debug.Log(CurrentScoreDataObserver.Value.Score);
    }

    public void IncrementJumpCount()
    {
        CurrentScoreDataObserver.Value.Jumps++;
        CurrentScoreDataObserver.Notify();
    }

    public void DecrementJumpCount()
    {
        CurrentScoreDataObserver.Value.Jumps--;
        CurrentScoreDataObserver.Notify();
    }
    public void UpdateDistanceTraveled(float updatedScore)
    {
        CurrentScoreDataObserver.Value.Distance = updatedScore;
        CurrentScoreDataObserver.Notify();
    }
    public Observer<ScoreData> CurrentScoreDataObserver{ get; } = new(new());
    public Observer<ScoreData> HighScoreDataObserver { get; } = new(new());

    public void Awake()
    {
        if (!File.Exists(HighScoreDataPath))
        {
            File.WriteAllText(HighScoreDataPath, JsonUtility.ToJson(new ScoreData()));
        }
        HighScoreDataObserver.Value = ReadFromPath();
        HighScoreDataObserver.Notify();
    }

    public void GameComplete()
    {
        ScoreData current = CurrentScoreDataObserver.Value;
        ScoreData score = HighScoreDataObserver.Value;
        if (current.Score > score.Score || (current.Score == score.Score && current.Jumps < score.Jumps))
        {
            HighScoreDataObserver.Value = new(current);
            SaveToPath(HighScoreDataObserver.Value);
        }
        CurrentScoreDataObserver.Value = new();
        CurrentScoreDataObserver.Notify();
    }
}