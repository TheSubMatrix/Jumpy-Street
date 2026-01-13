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
    
    static void SaveToPath(HighScoreData highScoreData) => File.WriteAllText(HighScoreDataPath, JsonUtility.ToJson(highScoreData));
    static HighScoreData ReadFromPath() => JsonUtility.FromJson<HighScoreData>(File.ReadAllText(HighScoreDataPath));
    
    public void UpdateScore(uint newScore)
    {
        CurrentScoreDataObserver.Value.Score = newScore;
        CurrentScoreDataObserver.Notify();
        Debug.Log($"Score: {CurrentScoreDataObserver.Value.Score}");
    }

    public void UpdateJumpCount(uint newJumpCount)
    {
        CurrentScoreDataObserver.Value.Jumps = newJumpCount;
        CurrentScoreDataObserver.Notify();
        Debug.Log($"Jumps: {CurrentScoreDataObserver.Value.Jumps}");
    }

    public Observer<HighScoreData> CurrentScoreDataObserver{ get; } = new(new());
    public Observer<HighScoreData> HighScoreDataObserver { get; } = new(new());

    public void Awake()
    {
        if (!File.Exists(HighScoreDataPath))
        {
            File.WriteAllText(HighScoreDataPath, JsonUtility.ToJson(new HighScoreData()));
        }
        HighScoreDataObserver.Value = ReadFromPath();
        HighScoreDataObserver.Notify();
    }

    public void GameComplete()
    {
        HighScoreData current = CurrentScoreDataObserver.Value;
        HighScoreData highScore = HighScoreDataObserver.Value;
        if (current.Score > highScore.Score || (current.Score == highScore.Score && current.Jumps < highScore.Jumps))
        {
            HighScoreDataObserver.Value = new(current);
        }
        SaveToPath(HighScoreDataObserver.Value);
    }

    public class HighScoreData
    {
        public uint Score;
        public uint Jumps;
        public HighScoreData(){}

        public HighScoreData(HighScoreData highScoreData)
        {
            Score = highScoreData.Score;
            Jumps = highScoreData.Jumps;
        }
    }
}