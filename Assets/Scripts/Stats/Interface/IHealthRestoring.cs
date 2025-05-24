namespace Stats.Interface
{
    public interface IHealthRestoring
    {
        bool IsHealthRestoringAfterHitEnemy { get; set; }
        bool IsHealthRestoringAfterDieEnemy { get; set; }
    }
}