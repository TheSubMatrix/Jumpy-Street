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
        CurrentScoreDataObserver.Value = new();
    }

    public class HighScoreData
    {
        public uint Score;
        public uint Jumps;
        public float Distance;
        public HighScoreData(){}

        public HighScoreData(HighScoreData highScoreData)
        {
            Score = highScoreData.Score;
            Jumps = highScoreData.Jumps;
            Distance = highScoreData.Distance;
        }
    }
}