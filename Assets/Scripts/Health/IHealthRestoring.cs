namespace Game.Core.Health
{
    public interface IHealthRestoring
    {
        bool IsHealthRestoringAfterHitEnemy { get; set; }
        bool IsHealthRestoringAfterDieEnemy { get; set; }
    }
}