using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OnceNavigator : WaypointNavigator
{
    bool m_hasCompleted = false;
    
    public override void Initialize(List<Vector3> waypoints)
    {
        base.Initialize(waypoints);
        m_hasCompleted = false;
    }
    
    public override void Advance()
    {
        CurrentIndex = NextIndex;
        
        if (NextIndex >= Waypoints.Count - 1)
        {
            m_hasCompleted = true;
        }
        else
        {
            NextIndex++;
        }
    }
    
    public override bool ShouldContinue() => !m_hasCompleted;
}