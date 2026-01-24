using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

public class ScorePickup : MonoBehaviour
{
    [Inject, UsedImplicitly]
    IScoreWriter m_scoreWriter;
    [Inject, UsedImplicitly]
    IScoreReader m_scoreReader;
    void OnTriggerEnter(Collider other)
    {
        m_scoreWriter.UpdateCurrentExtraPoints(m_scoreReader.GetHighScore().ExtraPoints + 1);
    }
}
