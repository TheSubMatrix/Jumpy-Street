public class ScoreData
{
    public uint Score;
    public uint Jumps;
    public float Distance;
    public ScoreData(){}

    public ScoreData(ScoreData scoreData)
    {
        Score = scoreData.Score;
        Jumps = scoreData.Jumps;
        Distance = scoreData.Distance;
    }
}