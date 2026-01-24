using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using MatrixUtils.GenericDatatypes;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour, IScoreWriter, IScoreReader, IDependencyProvider
{
    [Provide, UsedImplicitly]
    IScoreWriter GetScoreWriter() => this;
    
    [Provide, UsedImplicitly]
    IScoreReader GetHighScoreReader() => this;
    
    [SerializeReference, ClassSelector] IScoreRepository m_repository;

    readonly Observer<ScoreData> m_currentHighScore = new(new());
    readonly Observer<ScoreData> m_currentScore = new(new());
    void Awake()
    {
        m_currentHighScore.Value = m_repository.Load();
        m_currentHighScore.Notify();
    }

    public void UpdateCurrentDistance(float distance)
    {
        m_currentScore.Value.Distance = distance;
        m_currentScore.Notify();
    }

    public void UpdateCurrentExtraPoints(float extraPoints)
    {
        m_currentScore.Value.ExtraPoints = extraPoints;
        m_currentScore.Notify();
    }

    public void CommitScore()
    {
        if (m_currentScore.Value.Total > m_currentHighScore.Value.Total)
        {
            m_currentHighScore.Value = m_currentScore;
            m_repository.Save(m_currentHighScore);
            m_currentHighScore.Notify();
        }
        ResetScore();
    }

    public void ResetScore()
    {
        m_currentScore.Value = new();
    }
    
    public ScoreData GetHighScore() => m_currentHighScore;
    public ScoreData GetCurrentScore() => m_currentScore;
    public UnityEvent<ScoreData> OnHighScoreUpdated => m_currentHighScore.GetUnderlyingUnityEvent();
    public UnityEvent<ScoreData> OnCurrentScoreUpdated => m_currentScore.GetUnderlyingUnityEvent();
}