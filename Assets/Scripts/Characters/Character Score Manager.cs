using MatrixUtils.DependencyInjection;
using UnityEngine;

public class CharacterScoreManager : MonoBehaviour
{
    Vector3 m_startPosition;
    [Inject]
    IScoreManager m_scoreManager;

    void Awake()
    {
        ResetGame();
    }
    public void UpdateDistanceTraveled()
    {
        if ((uint)Mathf.RoundToInt(Vector3.Distance(m_startPosition, transform.position)) > (uint)Mathf.RoundToInt(m_scoreManager.CurrentScoreDataObserver.Value.Distance))
        {
            m_scoreManager.IncrementScore((uint)(Mathf.RoundToInt(Vector3.Distance(m_startPosition, transform.position)) - Mathf.RoundToInt(m_scoreManager.CurrentScoreDataObserver.Value.Distance)));
            m_scoreManager.UpdateDistanceTraveled(Vector3.Distance(m_startPosition, transform.position));
        }
    }

    public void IncrementJumpCount()
    {
        m_scoreManager.IncrementJumpCount();
    }
    
    public void GameComplete()
    {
        m_scoreManager.GameComplete();
    }

    public void ResetGame()
    {
        m_scoreManager.GameComplete();
        m_startPosition =  transform.position;     
    }
}
