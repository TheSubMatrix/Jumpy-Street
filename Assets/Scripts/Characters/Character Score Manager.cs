using MatrixUtils.DependencyInjection;
using UnityEngine;

public class CharacterScoreManager : MonoBehaviour
{
    Vector3 m_startPosition;
    uint m_jumpCount;
    
    [Inject]
    IScoreManager m_scoreManager;

    void Awake()
    {
        ResetGame();
    }
    public void UpdateScore()
    {
        m_scoreManager.UpdateScore((uint)Mathf.Max(0, Mathf.RoundToInt(Vector3.Distance(transform.position, m_startPosition))));
    }

    public void IncrementJumpCount()
    {
        m_scoreManager.UpdateJumpCount(++m_jumpCount);
    }
    
    public void GameComplete()
    {
        m_scoreManager.GameComplete();
    }

    public void ResetGame()
    {
        m_startPosition =  transform.position;
        UpdateScore();
        m_jumpCount = 0;
        m_scoreManager.UpdateJumpCount(m_jumpCount);
        
    }
}
