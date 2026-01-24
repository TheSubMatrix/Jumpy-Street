using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    IScoreReader m_scoreManager;
    [Inject, UsedImplicitly]
    void OnCurrentScoreManagerInjected(IScoreReader scoreManager)
    {
        m_scoreManager = scoreManager;
        m_scoreText.text = m_scorePrefix + m_scoreManager.GetCurrentScore().Total;
        m_highScoreText.text = m_scorePrefix + m_scoreManager.GetHighScore().Total;
        m_scoreManager.OnCurrentScoreUpdated.AddListener(OnScoreChanged);
        m_scoreManager.OnHighScoreUpdated.AddListener(OnHighScoreChanged);
    }
    [SerializeField] string m_scorePrefix = "Score: ";
    [SerializeField, RequiredField] TMP_Text m_scoreText;
    [SerializeField] string m_highScorePrefix = "High Score: ";
    [SerializeField, RequiredField] TMP_Text m_highScoreText;

    void OnScoreChanged(ScoreData currentScoreData)
    {
        m_scoreText.text = m_scorePrefix + currentScoreData.Total;
    }

    void OnHighScoreChanged(ScoreData scoreData)
    {
        m_highScoreText.text = m_highScorePrefix + scoreData.Total;
    }
}
