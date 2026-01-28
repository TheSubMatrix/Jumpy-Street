using JetBrains.Annotations;
using MatrixUtils.Attributes;
using MatrixUtils.DependencyInjection;
using TMPro;
using UnityEngine;

public class PlayAgainMenu : MonoBehaviour
{
    [Inject, UsedImplicitly]
    IScoreReader m_scoreReader;
    
    [RequiredField, SerializeField] TMP_Text m_score;
    [RequiredField, SerializeField] TMP_Text m_highScore;

    void Start()
    {
        m_score.text = "Score: " + m_scoreReader.GetLatestScore().Total;
        m_highScore.text = "High Score: " + m_scoreReader.GetHighScore().Total;
    }
}
