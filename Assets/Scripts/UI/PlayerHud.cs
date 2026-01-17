using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    IScoreManager m_scoreManager;
    [Inject, UsedImplicitly]
    void OnScoreManagerInjected(IScoreManager scoreManager)
    {
        m_scoreManager = scoreManager;
        m_scoreManager.CurrentScoreDataObserver.AddListener(OnScoreChanged);
        m_scoreManager.HighScoreDataObserver.AddListener(OnHighScoreChanged);
    }
    [SerializeField] string m_scorePrefix = "Score: ";
    [SerializeField, RequiredField] TMP_Text m_scoreText;
    [SerializeField] string m_highScorePrefix = "High Score: ";
    [SerializeField, RequiredField] TMP_Text m_highScoreText;

    void OnScoreChanged(ScoreData currentScoreData)
    {
        m_scoreText.text = m_scorePrefix + currentScoreData.Score;
    }

    void OnHighScoreChanged(ScoreData scoreData)
    {
        m_highScoreText.text = m_highScorePrefix + scoreData.Score;
    }
}
