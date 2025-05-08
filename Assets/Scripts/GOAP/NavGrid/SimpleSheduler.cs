using System.Diagnostics;

public class SimpleSheduler
{
    public readonly int MillisecondsPerFrame;
    private readonly Stopwatch sw = new Stopwatch();

    public SimpleSheduler(int millisecondsPerFrame)
    {
        MillisecondsPerFrame = millisecondsPerFrame;
    }

    public void Start()
    {
        sw.Restart();
    }

    public bool IsEnd()
    {
        return sw.ElapsedMilliseconds >= MillisecondsPerFrame;
    }
}
