using System;

[Serializable]
public class PingPongNavigator : WaypointNavigator
{
    bool m_movingForward = true;
    
    public override void Advance()
    {
        CurrentIndex = NextIndex;
        
        if (m_movingForward)
        {
            if (NextIndex >= Waypoints.Count - 1)
            {
                m_movingForward = false;
                NextIndex = CurrentIndex - 1;
            }
            else
            {
                NextIndex++;
            }
        }
        else
        {
            if (NextIndex <= 0)
            {
                m_movingForward = true;
                NextIndex = CurrentIndex + 1;
            }
            else
            {
                NextIndex--;
            }
        }
    }
    
    public override bool ShouldContinue() => true;
}