using MatrixUtils.DependencyInjection;
using UnityEngine;

public class PlayerScoreProxy : MonoBehaviour
{
    [Inject] IScoreWriter m_scoreWriter;
    Vector3 m_startPosition;

    void Awake()
    {
        m_startPosition = transform.position;
    }
    public void UpdateDistance()
    {
        m_scoreWriter.UpdateCurrentDistance(Vector3.Distance(m_startPosition, transform.position));
    }

    public void FinalizeScore()
    {
        m_scoreWriter.CommitScore();
    }
}
