using MatrixUtils.DependencyInjection;
using UnityEngine;

public interface IScoreWriter
{
    void UpdateCurrentDistance(float distance);
    void UpdateCurrentExtraPoints(float extraPoints);
    void CommitScore();
    void ResetScore();
}