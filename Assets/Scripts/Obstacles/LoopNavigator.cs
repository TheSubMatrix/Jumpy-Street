using System;

[Serializable]
public class LoopNavigator : WaypointNavigator
{
    public override void Advance()
    {
        CurrentIndex = NextIndex;
        NextIndex = (NextIndex + 1) % Waypoints.Count;
    }
    public override bool ShouldContinue() => true;
}