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
        float currentDistance = Vector3.Distance(m_startPosition, transform.position);
        uint currentDistanceRounded = (uint)Mathf.RoundToInt(currentDistance);
        uint storedDistance = (uint)Mathf.RoundToInt(m_scoreManager.CurrentScoreDataObserver.Value.Distance);
        if (currentDistanceRounded <= storedDistance) return;
        m_scoreManager.IncrementScore(currentDistanceRounded - storedDistance);
        m_scoreManager.UpdateDistanceTraveled(currentDistance);
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
