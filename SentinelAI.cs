using UnityEngine;

namespace SentinelProtocol
{
    public enum SentinelState { Patrol, Investigate, Chase, Attack }

    [RequireComponent(typeof(EnemySensor), typeof(EnemyNavigator), typeof(Health))]
    public class SentinelAI : MonoBehaviour
    {
        [SerializeField] private float investigateDuration = 4f;
        [SerializeField] private float attackRange = 1.8f;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackCooldown = 1.2f;

        private EnemySensor sensor;
        private EnemyNavigator navigator;
        private Health health;
        private Health playerHealth;
        private Transform player;
        private Vector3 lastKnownPosition;
        private float investigateUntil;
        private float nextAttackTime;

        public SentinelState State { get; private set; } = SentinelState.Patrol;
        public Vector3 LastKnownPosition => lastKnownPosition;
        public EnemySensor Sensor => sensor;

        private void Awake()
        {
            sensor = GetComponent<EnemySensor>();
            navigator = GetComponent<EnemyNavigator>();
            health = GetComponent<Health>();
            health.Died += OnDied;
        }

        public void Initialize(Transform target)
        {
            player = target;
            playerHealth = target ? target.GetComponent<Health>() : null;
            if (sensor) sensor.Target = target;
        }

        private void Update()
        {
            if (health.IsDead) return;

            bool seesPlayer = sensor && sensor.CanSeeTarget();
            if (seesPlayer)
            {
                lastKnownPosition = player.position;
                State = Vector3.Distance(transform.position, player.position) <= attackRange
                    ? SentinelState.Attack : SentinelState.Chase;
            }
            else if (State == SentinelState.Chase || State == SentinelState.Attack)
            {
                State = SentinelState.Investigate;
                investigateUntil = Time.time + investigateDuration;
                navigator.MoveTo(lastKnownPosition);
            }

            switch (State)
            {
                case SentinelState.Patrol:
                    navigator.Patrol();
                    break;
                case SentinelState.Investigate:
                    navigator.MoveTo(lastKnownPosition);
                    if (navigator.ReachedDestination() || Time.time >= investigateUntil)
                        State = SentinelState.Patrol;
                    break;
                case SentinelState.Chase:
                    if (player) navigator.MoveTo(player.position);
                    break;
                case SentinelState.Attack:
                    if (player) navigator.MoveTo(player.position);
                    if (Time.time >= nextAttackTime && playerHealth && !playerHealth.IsDead)
                    {
                        playerHealth.TakeDamage(attackDamage);
                        nextAttackTime = Time.time + attackCooldown;
                    }
                    break;
            }
        }

        private void OnDied()
        {
            State = SentinelState.Patrol;
            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent) agent.isStopped = true;
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
            Destroy(gameObject, 2f);
        }

        private void OnDestroy()
        {
            if (health) health.Died -= OnDied;
        }
    }
}
