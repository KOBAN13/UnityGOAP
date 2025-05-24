namespace Health.Interface
{
    public interface IHealthRestoring
    {
        bool IsHealthRestoringAfterHitEnemy { get; set; }
        bool IsHealthRestoringAfterDieEnemy { get; set; }
    }
}