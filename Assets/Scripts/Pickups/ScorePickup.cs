using JetBrains.Annotations;
using MatrixUtils.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

public class ScorePickup : MonoBehaviour
{
    [Inject, UsedImplicitly]
    IScoreManager scoreManager;

    private void OnTriggerEnter(Collider other)
    {
        scoreManager.IncrementScore();
    }
}
