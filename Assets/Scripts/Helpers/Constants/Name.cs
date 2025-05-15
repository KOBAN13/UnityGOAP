namespace Helpers.Constants
{
    public class NameAIKeys
    {
        public static readonly string Animator = "AIAnimator";
        public static readonly string Agent = "NavMeshAgent";
        public static readonly string HealthAI = "Health";
        public static readonly string FoodPoint = "FoodPoint";
        public static readonly string ChillPoint = "ChillPoint";
        public static readonly string TransformAI = "Transform";
        public static readonly string PlayerTarget = "Player";
        public static readonly string NavGrid = "NavGrid";
        public static readonly string SearchEnemyRadius = "Radius";
        public static readonly string TimeToSearchEnemy = "TimeSearch";
        public static readonly string CountIterationSearchEnemy = "CountIteration";
    }

    public class NameExperts
    {
        public static string MovementPredicate => nameof(MovementPredicate);
        public static string HealthPredicate => nameof(HealthPredicate);
        public static string StaminaPredicate => nameof(StaminaPredicate);
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
    
    public class AgentBeliefsName
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
        
        public static string Chill => nameof(Chill);
        public static string Walk => nameof(Walk);
        public static string MoveToEat => nameof(MoveToEat);
        public static string Heal => nameof(Heal);
        public static string PlayerLook => nameof(PlayerLook);
        public static string PlayerHit => nameof(PlayerHit);
        public static string PlayerEscaped => nameof(PlayerEscaped);
        public static string PlayerAttacked => nameof(PlayerAttacked);
    }
}