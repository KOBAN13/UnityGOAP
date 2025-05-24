namespace Helpers.Constants
{
    public static class NameAIKeys
    {
        public static string AnimationBrain => nameof(AnimationBrain);
        public static string Agent => nameof(Agent);
        public static string AgentStats => nameof(AgentStats);
        public static string FoodLocation => nameof(FoodLocation);
        public static string ChillLocate => nameof(ChillLocate);
        public static string AgentTransform => nameof(AgentTransform);
        public static string PlayerTarget => nameof(PlayerTarget);
        public static string NavGrid => nameof(NavGrid);
        public static string SearchEnemyRadius => nameof(SearchEnemyRadius);
        public static string TimeToSearchEnemy => nameof(TimeToSearchEnemy);
        public static string CountIterationSearchEnemy => nameof(CountIterationSearchEnemy);
    }

    public static class NameAgentPredicate
    {
        public static string MovementPredicate => nameof(MovementPredicate);
        public static string HealthPredicate => nameof(HealthPredicate);
        public static string StaminaPredicate => nameof(StaminaPredicate);
        public static string StaminaLowPredicate => nameof(StaminaLowPredicate);
        public static string LocationFoodPredicate => nameof(LocationFoodPredicate);
        public static string LocationChillPredicate => nameof(LocationChillPredicate);
        public static string EyesSensor => nameof(EyesSensor);
        public static string HitSensor => nameof(HitSensor);
        public static string AttackSensor => nameof(AttackSensor);
        public static string EnemyVisionSensor => nameof(EnemyVisionSensor);
        public static string NothingPredicate => nameof(NothingPredicate);
        public static string AttackPredicate => nameof(AttackPredicate);
        public static string IdlePredicate => nameof(IdlePredicate);
        public static string HealthLowPredicate => nameof(HealthLowPredicate);
        public static string HitSensorPredicate => nameof(HitSensorPredicate);
        public static string AttackSensorPredicate => nameof(AttackSensorPredicate);
        public static string VisionSensorPredicate => nameof(VisionSensorPredicate);
        public static string EyesSensorPredicate => nameof(EyesSensorPredicate);
    }
    
    public static class AgentBeliefsName
    {
        public static string Nothing => nameof(Nothing);
        public static string AttackingPlayer => nameof(AttackingPlayer);
        public static string AgentIdle => nameof(AgentIdle);
        public static string AgentMoving => nameof(AgentMoving);
        public static string AgentIsHealthLow => nameof(AgentIsHealthLow);
        public static string AgentIsHealthy => nameof(AgentIsHealthy);
        public static string AgentAtFoodPosition => nameof(AgentAtFoodPosition);
        public static string AgentAtRestingPosition => nameof(AgentAtRestingPosition);
        public static string PlayerInEyeSensor => nameof(PlayerInEyeSensor);
        public static string PlayerToAttackSensor => nameof(PlayerToAttackSensor);
        public static string PlayerInHitSensor => nameof(PlayerInHitSensor);
        public static string EnemySearch => nameof(EnemySearch);
    }
}