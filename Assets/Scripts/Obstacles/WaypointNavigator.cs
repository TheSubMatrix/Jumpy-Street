using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class WaypointNavigator
{
    protected List<Vector3> Waypoints;
    protected int CurrentIndex = 0;
    protected int NextIndex = 1;
    
    public Vector3 CurrentWaypoint => Waypoints[CurrentIndex];
    public Vector3 NextWaypoint => Waypoints[NextIndex];
    
    public virtual void Initialize(List<Vector3> waypoints)
    {
        Waypoints = waypoints;
        CurrentIndex = 0;
        NextIndex = 1;
    }
    
    public abstract void Advance();
    public abstract bool ShouldContinue();
}